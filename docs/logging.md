# Руководство по системе логирования

## Обзор

В проекте используется стек **ELK** (Elasticsearch, Logstash, Kibana) для централизованного сбора и анализа логов со всех сервисов.
Filebeat используется для сбора логов из контейнеров и передачи их в Logstash.

## Основные компоненты

- **Elasticsearch**: хранилище логов и поисковый движок
- **Logstash**: обработка и фильтрация логов
- **Kibana**: веб-интерфейс для визуализации и анализа логов
- **Filebeat**: агент для сбора логов

## Доступ к Kibana

Kibana доступна по URL: https://kibana.bugtracker.nasekinid.ru

## Типичные сценарии использования

### Поиск логов конкретного сервиса

1. Откройте Kibana и перейдите в раздел "Discover"
2. Выберите индекс `logstash-*`
3. Используйте следующий запрос для поиска логов определенного сервиса:
   ```
   kubernetes.container.name: "user-service"
   ```

### Поиск ошибок в логах

Для поиска ошибок используйте:

```
log: "*error*" OR level: "error" OR level: "Error"
```

### Поиск логов по времени

Используйте временную шкалу в верхней части экрана Kibana для выбора временного диапазона.

### Создание дашбордов

1. Перейдите в раздел "Dashboard"
2. Нажмите "Create dashboard"
3. Добавьте визуализации с помощью "Add"

## Структура логов

Каждая запись лога содержит следующие основные поля:

- `@timestamp`: время события
- `kubernetes.container.name`: имя контейнера
- `kubernetes.pod.name`: имя пода
- `kubernetes.namespace`: пространство имен
- `log`: текст сообщения
- `level`: уровень логирования (если доступен)

## Устранение неполадок с Kibana

### Ошибка 500 (Internal Server Error)

Если Kibana выдает ошибку 500, проверьте:

1. Состояние подов в кластере:

   ```
   kubectl get pods -n bugtracker | grep kibana
   kubectl describe pod <имя-пода-kibana> -n bugtracker
   kubectl logs <имя-пода-kibana> -n bugtracker
   ```

2. Проверьте подключение к Elasticsearch:

   ```
   kubectl get pods -n bugtracker | grep elasticsearch
   kubectl logs <имя-пода-elasticsearch> -n bugtracker
   ```

3. Проверьте настройки индексов:

   ```
   kubectl exec -it <имя-пода-elasticsearch> -n bugtracker -- curl localhost:9200/_cat/indices
   ```

4. Перезапустите под Kibana:
   ```
   kubectl delete pod <имя-пода-kibana> -n bugtracker
   ```

### Если логи не отображаются

1. Проверьте состояние Filebeat:

   ```
   kubectl get pods -n bugtracker | grep filebeat
   kubectl logs <имя-пода-filebeat> -n bugtracker
   ```

2. Проверьте состояние Logstash:
   ```
   kubectl get pods -n bugtracker | grep logstash
   kubectl logs <имя-пода-logstash> -n bugtracker
   ```
