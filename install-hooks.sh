#!/bin/bash
# Копирование хуков из .githooks в .git/hooks
cp .githooks/* .git/hooks/
chmod +x .git/hooks/*
echo "Git hooks установлены."
