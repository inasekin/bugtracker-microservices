#!/bin/bash

# Скрипт для очистки старых индексов в Elasticsearch
# Использование: ./cleanup-indices.sh [количество дней для хранения]

# Количество дней хранения (по умолчанию 7)
DAYS_TO_KEEP=${1:-7}

# Определение текущей даты и формирование даты отсечения
# Проверка ОС и использование подходящей команды date
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS использует другой формат команды date
    CUTOFF_DATE=$(date -v -${DAYS_TO_KEEP}d +%Y.%m.%d)
else
    # Linux формат
    CUTOFF_DATE=$(date -d "now - $DAYS_TO_KEEP days" +%Y.%m.%d)
fi

echo "Начало очистки индексов старше $CUTOFF_DATE"

# Получение списка индексов и фильтрация старых
INDICES=$(curl -s -X GET "localhost:9200/_cat/indices?h=index" | grep -E '.*-[0-9]{4}\.[0-9]{2}\.[0-9]{2}')

for INDEX in $INDICES; do
    # Извлечение даты из имени индекса
    INDEX_DATE=$(echo $INDEX | grep -oE '[0-9]{4}\.[0-9]{2}\.[0-9]{2}')
    
    # Проверка, имеет ли индекс формат даты
    if [ -n "$INDEX_DATE" ]; then
        # Сравнение даты индекса с датой отсечения
        if [[ "$INDEX_DATE" < "$CUTOFF_DATE" ]]; then
            echo "Удаление индекса: $INDEX (дата: $INDEX_DATE)"
            curl -s -X DELETE "localhost:9200/$INDEX"
            echo ""
        fi
    fi
done

# Сброс блокировки "только для чтения", если она установлена
echo "Сброс блокировки 'только для чтения' со всех индексов"
curl -s -X PUT "localhost:9200/_all/_settings" -H 'Content-Type: application/json' -d'{
  "index.blocks.read_only_allow_delete": null
}'

echo "Очистка индексов завершена" 