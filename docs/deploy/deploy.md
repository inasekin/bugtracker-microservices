# BugTracker в Kubernetes на Ubuntu в Яндекс.Облаке

## Содержание

- [Обзор](#обзор)
- [Шаг 1: Создание виртуальной машины](#шаг-1-создание-виртуальной-машины)
- [Шаг 2: Установка Kubernetes (K3s)](#шаг-2-установка-kubernetes-k3s)
- [Шаг 3: Установка графического интерфейса для Kubernetes](#шаг-3-установка-графического-интерфейса-для-kubernetes)
- [Шаг 4: Подготовка конфигурационных файлов](#шаг-4-подготовка-конфигурационных-файлов)

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

### Установка Nginx Ingress Controller

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
02-mongodb - база данных должна быть готова до запуска сервисов
03-services - микросервисы развертываются после базы данных
04-gateway - API-шлюз разворачивается после микросервисов
05-frontend - фронтенд разворачивается после бэкенда
06-ingress - настройка внешнего входа должна быть последней
```

Далее создаем все необходимые конфиги в нужных директориях
