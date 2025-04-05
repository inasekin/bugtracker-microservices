# Обслуживание ELK-стека

## Компоненты стека

- **Elasticsearch**: хранилище данных и поисковый движок
- **Logstash**: обработка и фильтрация логов
- **Kibana**: визуализация и анализ данных
- **Filebeat**: сбор логов из контейнеров

## Управление дисковым пространством

### Настройки Elasticsearch

В файле `docker/docker-compose.elk.yml` настроены следующие параметры для предотвращения блокировки индексов:

```yaml
- cluster.routing.allocation.disk.watermark.low=85%
- cluster.routing.allocation.disk.watermark.high=90%
- cluster.routing.allocation.disk.watermark.flood_stage=95%
```

Эти настройки определяют, когда Elasticsearch начнет переносить шарды (низкий уровень), блокировать создание новых индексов (высокий уровень) и переводить индексы в режим "только для чтения" (критический уровень).

### Автоматическая очистка старых индексов

Для автоматической очистки старых индексов можно использовать скрипт `docker/elasticsearch/cleanup-indices.sh`:

```bash
# Очистка индексов старше 7 дней (по умолчанию)
./docker/elasticsearch/cleanup-indices.sh

# Очистка индексов старше 3 дней
./docker/elasticsearch/cleanup-indices.sh 3
```

### Ручная очистка индексов

Для ручной очистки индексов:

1. Посмотреть текущие индексы по размеру:

   ```bash
   curl -X GET "localhost:9200/_cat/indices?v&s=store.size:desc" | head -20
   ```

2. Удалить старые или ненужные индексы:

   ```bash
   curl -X DELETE "localhost:9200/logstash-YYYY.MM.DD"
   ```

3. После очистки сбросить блокировку "только для чтения":
   ```bash
   curl -X PUT "localhost:9200/_all/_settings" -H 'Content-Type: application/json' -d'{
     "index.blocks.read_only_allow_delete": null
   }'
   ```

## Настройка Logstash

Конфигурация Logstash находится в файле `docker/logstash/pipeline/logstash.conf`. Если необходимо изменить обработку логов:

1. Отредактируйте файл конфигурации
2. Перезапустите контейнер Logstash:
   ```bash
   docker compose -f docker/docker-compose.elk.yml restart logstash
   ```

### Текущая настройка фильтрации логов

В текущей конфигурации Logstash настроена фильтрация логов по сервисам:

- userservice
- projectservice
- commentsservice
- gatewayservice
- прочие сервисы (microservices)

## Диагностика проблем

### Проверка состояния кластера Elasticsearch

```bash
curl -X GET "localhost:9200/_cluster/health?pretty"
```

Статус может быть:

- `green`: все шарды доступны
- `yellow`: все первичные шарды доступны, но некоторые реплики недоступны
- `red`: некоторые первичные шарды недоступны

### Решение проблемы с сохранением Data View в Kibana

Если при создании Data View (шаблона индекса) в Kibana появляется ошибка "Failed to save data view", это может быть вызвано следующими причинами:

1. **Индексы Kibana заблокированы в режиме "только для чтения"**:

   ```bash
   # Снимите блокировку с индексов Kibana
   curl -X PUT "localhost:9200/.kibana*/_settings" -H 'Content-Type: application/json' -d'{"index.blocks.read_only_allow_delete": null}'
   ```

2. **Переполнение диска**:

   ```bash
   # Удалите старые индексы
   ./docker/elasticsearch/cleanup-indices.sh 5  # хранить логи за последние 5 дней
   ```

3. **Создание Data View через API**:

   ```bash
   # Создать индекс-паттерн программно
   curl -X POST "localhost:5601/api/saved_objects/index-pattern" -H "kbn-xsrf: true" -H "Content-Type: application/json" -d'{"attributes":{"title":"*service*","timeFieldName":"@timestamp"}}'
   ```

4. **Перезапуск Kibana**:
   ```bash
   docker compose -f docker/docker-compose.elk.yml restart kibana
   ```

### Решение проблемы с бесконечной загрузкой страницы services в Kibana

Если при открытии страницы services или других индекс-паттернов в Kibana происходит бесконечная загрузка, это может быть вызвано следующими причинами:

1. **Повреждённые индексы в состоянии "red"**:

   ```bash
   # Проверка индексов в состоянии "red"
   curl -X GET "localhost:9200/_cat/indices?v&health=red"

   # Удаление проблемных индексов
   curl -X DELETE "localhost:9200/services"
   curl -X DELETE "localhost:9200/microservices-YYYY.MM.DD"
   ```

2. **Проблемы с индекс-паттернами в Kibana**:

   ```bash
   # Проверка существующих индекс-паттернов
   curl -X GET "localhost:5601/api/saved_objects/_find?type=index-pattern&per_page=100" -H "kbn-xsrf: true"

   # Удаление проблемного индекс-паттерна (заменить ID на актуальный)
   curl -X DELETE "localhost:5601/api/saved_objects/index-pattern/YOUR_INDEX_PATTERN_ID" -H "kbn-xsrf: true"

   # Создание индекс-паттернов для каждого сервиса отдельно
   curl -X POST "localhost:5601/api/saved_objects/index-pattern" -H "kbn-xsrf: true" -H "Content-Type: application/json" -d'{"attributes":{"title":"userservice-*","timeFieldName":"@timestamp"}}'
   curl -X POST "localhost:5601/api/saved_objects/index-pattern" -H "kbn-xsrf: true" -H "Content-Type: application/json" -d'{"attributes":{"title":"projectservice-*","timeFieldName":"@timestamp"}}'
   curl -X POST "localhost:5601/api/saved_objects/index-pattern" -H "kbn-xsrf: true" -H "Content-Type: application/json" -d'{"attributes":{"title":"commentsservice-*","timeFieldName":"@timestamp"}}'
   curl -X POST "localhost:5601/api/saved_objects/index-pattern" -H "kbn-xsrf: true" -H "Content-Type: application/json" -d'{"attributes":{"title":"gatewayservice-*","timeFieldName":"@timestamp"}}'
   ```

3. **После внесения изменений перезапустите Kibana**:

   ```bash
   docker compose -f docker/docker-compose.elk.yml restart kibana
   ```

4. **Если проблема сохраняется, очистите кеш браузера** или попробуйте открыть Kibana в режиме инкогнито.

### Проверка логов контейнеров

```bash
# Логи Elasticsearch
docker logs elasticsearch

# Логи Kibana
docker logs kibana

# Логи Logstash
docker logs logstash

# Логи Filebeat
docker logs filebeat
```

### Проверка подключения Filebeat к Logstash

```bash
# Проверка TCP-соединения на порт 5044
nc -vz localhost 5044
```

## Обновление компонентов

При обновлении версий компонентов ELK-стека в `docker/docker-compose.elk.yml`, всегда используйте одинаковые версии для всех компонентов, чтобы избежать проблем совместимости.

Текущая версия: 8.11.1

## Резервное копирование

Для резервного копирования данных Elasticsearch:

```bash
# Создание снапшота индексов (требует настройки репозитория снапшотов)
curl -X PUT "localhost:9200/_snapshot/my_backup/snapshot_1?wait_for_completion=true"
```

## Мониторинг

Для мониторинга ELK-стека рекомендуется использовать:

1. Встроенный мониторинг Kibana (Stack Monitoring)
2. Метрики Elasticsearch: `curl -X GET "localhost:9200/_nodes/stats?pretty"`
3. Мониторинг дискового пространства: `curl -X GET "localhost:9200/_cat/allocation?v"`

```

```
