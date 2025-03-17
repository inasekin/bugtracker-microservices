# BugTracker в Kubernetes на Ubuntu в Яндекс.Облаке

## Содержание

- [Обзор](#обзор)
- [Шаг 1: Создание виртуальной машины](#шаг-1-создание-виртуальной-машины)
- [Шаг 2: Установка Kubernetes (K3s)](#шаг-2-установка-kubernetes-k3s)
- [Шаг 3: Установка графического интерфейса для Kubernetes](#шаг-3-установка-графического-интерфейса-для-kubernetes)
- [Шаг 4: Подготовка конфигурационных файлов](#шаг-4-подготовка-конфигурационных-файлов)
- [Шаг 5: Настройка доступа к GitHub Packages](#шаг-5-настройка-доступа-к-github-packages)
- [Шаг 6: Настройка домена](#шаг-6-настройка-домена)
- [Шаг 7: Применение конфигураций Kubernetes](#шаг-7-применение-конфигураций-kubernetes)

## Обзор

### Почему этот подход?

- Более бюджетное решение, чем Managed Kubernetes от Яндекс.Облака
- Полный контроль над кластером
- Легкая настройка и управление через K3s
- Достаточно для тестирования и малой/средней нагрузки

### Архитектура решения

- Ubuntu 20.04 LTS на виртуальной машине в Яндекс.Облаке
- K3s (легковесный Kubernetes) для оркестрации контейнеров
- Nginx Ingress Controller для маршрутизации внешнего трафика
- Микросервисы развернуты как Deployments в Kubernetes
- MongoDB для хранения данных

## Шаг 1: Создание виртуальной машины

### Установка и настройка Yandex.Cloud CLI

```bash
# Установка CLI
curl https://storage.yandexcloud.net/yandexcloud-yc/install.sh | bash
source ~/.bashrc

# Инициализация и настройка
yc init
```

### Создание виртуальной машины

```bash
# Создаем виртуальную машину с Ubuntu 20.04
yc compute instance create \
  --name bugtracker-vm \
  --zone ru-central1-a \
  --public-ip \
  --ssh-key ~/.ssh/id_rsa.pub \
  --memory 4 \
  --cores 2 \
  --core-fraction 50 \
  --preemptible \
  --create-boot-disk image-id=fd8ingbofbh3j5h7i8ll,size=15
```

### Подключение к виртуальной машине

```bash
# Подключаемся по SSH
ssh -l yc-user <IP-АДРЕС-VM>
```

## Шаг 2: Установка Kubernetes (K3s)

На виртуальной машине выполните:

```bash
# Обновление системы
sudo apt update && sudo apt upgrade -y

# Установка необходимых пакетов
sudo apt install -y curl apt-transport-https ca-certificates software-properties-common

# Отключаем своп (рекомендуется для Kubernetes)
sudo swapoff -a
sudo sed -i '/ swap / s/^\(.*\)$/#\1/g' /etc/fstab

# Установка K3s
curl -sfL https://get.k3s.io | sh -

# Настройка доступа
mkdir -p $HOME/.kube
sudo cp /etc/rancher/k3s/k3s.yaml $HOME/.kube/config
sudo chown $(id -u):$(id -g) $HOME/.kube/config
export KUBECONFIG=$HOME/.kube/config
echo "export KUBECONFIG=$HOME/.kube/config" >> $HOME/.bashrc

# Проверка работы K3s, долден выдать список зарегистрированной ноды
kubectl get nodes
```

### Установка Nginx Ingress Controller + Cert Manager

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.1/deploy/static/provider/baremetal/deploy.yaml

# Проверка установки, долден выдать список зарегистрированных подов
kubectl get pods -n ingress-nginx
```

## Шаг 3: Установка графического интерфейса для Kubernetes

Kubernetes Dashboard:

```bash
# Установка Kubernetes Dashboard
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml

# Создание пользователя с правами администратора
cat <<EOF | kubectl apply -f -
apiVersion: v1
kind: ServiceAccount
metadata:
  name: admin-user
  namespace: kubernetes-dashboard
---
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: admin-user
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: cluster-admin
subjects:
- kind: ServiceAccount
  name: admin-user
  namespace: kubernetes-dashboard
EOF

# Получение токена для входа
kubectl -n kubernetes-dashboard create token admin-user

# Запуск прокси для доступа к дашборду
kubectl proxy
```

После выполнения этих команд dashboard будет доступен по адресу:
http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/

Для доступа извне виртуальной машины, настройте SSH-туннель на вашем локалке:
```bash
ssh -L 8001:localhost:8001 yc-user@<IP-АДРЕС-VM>
```

## Шаг 4: Подготовка конфигурационных файлов

Создаем структуру папок для организованного хранения конфигурационных файлов Kubernetes.

Применяем практику "Упорядоченность развертывания" (каждая группа манифестов находится в отдельной папке с префиксом-номером, что указывает очередность их применения).

```bash
# Создаем структуру директорий для Kubernetes
mkdir -p kubernetes/manifests/{00-namespace,01-configs,02-mongodb,03-services,04-gateway,05-frontend,06-ingress}

# Пояснение к структуре
00-namespace - создание пространства имен должно происходить первым
01-configs - конфигурации и секреты должны быть созданы до компонентов, которые их используют
02-databses - базы данных должна быть готовы до запуска сервисов
03-services - микросервисы развертываются после базы данных
04-gateway - API-шлюз разворачивается после микросервисов
05-frontend - фронтенд разворачивается после бэкенда
06-ingress - настройка внешнего входа должна быть последней
```

Далее создаем все необходимые конфиги в нужных директориях

## Шаг 5: Настройка доступа к GitHub Packages

```bash
# Создаем namespace
kubectl create namespace bugtracker

# Создаем секрет для доступа к GitHub Packages
kubectl create secret docker-registry github-registry \
  --namespace bugtracker \
  --docker-server=ghcr.io \
  --docker-username=<GITHUB_USERNAME> \
  --docker-password=<GITHUB_TOKEN>
```
## Шаг 6: Настройка домена

Для автоматической выдачи и обновления SSL-сертификатов используем cert-manager:

```bash
# Установка cert-manager
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.12.0/cert-manager.yaml

kubectl -n cert-manager wait --for=condition=ready pod -l app.kubernetes.io/instance=cert-manager --timeout=120s
```

Чтобы связать домен с виртуальной машиной в Яндекс.Облаке:

Узнайте внешний IP вашей виртуальной машины:
```bash
IP_ADDRESS=$(curl -s ifconfig.me)
echo $IP_ADDRESS

Войдите в панель управления провайдера и найдите раздел управления DNS для вашего домена
Добавьте запись типа A:

Имя: bugtracker (или другой желаемый поддомен)
Тип: A
Значение: IP-адрес вашей виртуальной машины
```

## Шаг 7: Применение конфигураций Kubernetes

На сервер облака необходимо скачать репозиторий с конфигурацией

Для автоматического нужно использовать скрипт deploy.sh

```bash
# Установка прав на выполнение скрипта
chmod +x deploy.sh

# Запуск скрипта с необходимыми параметрами
./deploy.sh <GITHUB_USERNAME> <GITHUB_TOKEN> <DOMAIN> <EMAIL>
```

Где:

<GITHUB_USERNAME> - ваше имя пользователя на GitHub
<GITHUB_TOKEN> - токен с правами доступа к пакетам (read:packages)
<DOMAIN> - ваш основной домен (например, yourdomain.ru)
<EMAIL> - ваша электронная почта для сертификатов Let's Encrypt

Или вы можете применить конфигурации вручную:

Обращайте внимание на путь к конфигам!

```bash

# 1. Namespace
kubectl apply -f kubernetes/manifests/00-namespace/namespace.yaml

# 2. Конфигурации и секреты
kubectl apply -f kubernetes/manifests/01-configs/

# 3. Базы данных (MongoDB, PostgreSQL, Redis, RabbitMQ)
kubectl apply -f kubernetes/manifests/02-databases/

# 4. Микросервисы
kubectl apply -f kubernetes/manifests/03-services/

# 5. API Gateway
kubectl apply -f kubernetes/manifests/04-gateway/

# 6. Frontend
kubectl apply -f kubernetes/manifests/05-frontend/

# 7. Cert-manager и Ingress
kubectl apply -f kubernetes/manifests/06-ingress/
```
