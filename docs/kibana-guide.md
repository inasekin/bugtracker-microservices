# Руководство по просмотру логов в Kibana

## Доступ к Kibana

Kibana доступна по адресу: http://localhost:5601

## Настройка индекса в Kibana

При первом входе в Kibana необходимо настроить индексы:

1. Перейдите в раздел "Stack Management" (слева внизу) -> "Index Patterns"
2. Нажмите "Create index pattern"
3. Введите следующий шаблон индекса: `*service*` (это отобразит все индексы сервисов)
4. Выберите `@timestamp` как поле времени
5. Нажмите "Create index pattern"

### Если не удается сохранить индекс-паттерн (Failed to save data view)

Если при создании индекс-паттерна возникает ошибка, выполните следующие действия:

1. Снимите блокировку "только для чтения" с индексов Kibana:

   ```bash
   curl -X PUT "localhost:9200/.kibana*/_settings" -H 'Content-Type: application/json' -d'{"index.blocks.read_only_allow_delete": null}'
   ```

2. Создайте индекс-паттерн через API:

   ```bash
   curl -X POST "localhost:5601/api/saved_objects/index-pattern" -H "kbn-xsrf: true" -H "Content-Type: application/json" -d'{"attributes":{"title":"*service*","timeFieldName":"@timestamp"}}'
   ```

3. Перезагрузите страницу Kibana и проверьте, что индекс-паттерн создан

## Просмотр логов UserService

1. Перейдите в раздел "Discover" в левом меню
2. Выберите созданный индекс-паттерн
3. Настройте временной диапазон в верхнем правом углу
4. В поисковой строке используйте следующие запросы:

```
service_name: "userservice"
```

или для поиска логов регистрации:

```
service_name: "userservice" AND log_message: *регистрация*
```

или

```
service_name: "userservice" AND log_message: *register*
```

## Просмотр логов других сервисов

Аналогично для других сервисов:

```
service_name: "projectservice"
```

```
service_name: "commentsservice"
```

```
service_name: "gatewayservice"
```

## Поиск по уровню логирования

Для поиска ошибок используйте:

```
log_level: "Error" OR log_level: "ERROR"
```

Для информационных сообщений:

```
log_level: "Information" OR log_level: "INFO"
```

## Сохранение поисковых запросов

Чтобы сохранить часто используемый поисковый запрос:

1. Настройте поисковый запрос
2. Нажмите "Save" в верхнем меню
3. Введите имя для сохраненного поиска
4. Нажмите "Save"

## Создание дашборда

Для создания дашборда:

1. Перейдите в раздел "Dashboard"
2. Нажмите "Create dashboard"
3. Нажмите "Create visualization"
4. Выберите тип визуализации (например, "Line", "Bar" или "Data Table")
5. Настройте визуализацию и добавьте на дашборд
6. Сохраните дашборд

## Устранение неполадок

### Если индекс заблокирован (read-only)

Если Elasticsearch заблокировал индексы из-за заполнения диска:

```bash
# Удаление старых индексов
curl -X DELETE "localhost:9200/logstash-СТАРАЯ_ДАТА"

# Сброс блокировки "только для чтения"
curl -X PUT "localhost:9200/_all/_settings" -H 'Content-Type: application/json' -d'
{
  "index.blocks.read_only_allow_delete": null
}'
```

### Если логи отсутствуют

1. Убедитесь, что сервисы работают и генерируют логи
2. Проверьте, что Filebeat собирает логи
3. Проверьте, что Logstash правильно обрабатывает логи
4. Проверьте наличие индексов: `curl -X GET "localhost:9200/_cat/indices?v"`
