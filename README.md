# Forum

Контрольная работа №8: ASP.NET Core MVC форум с SQLite, Identity и AJAX.

## Запуск

```powershell
dotnet restore
dotnet run
```

База создается миграцией при первом запуске. Демонстрационные аккаунты:

- `anna_forum` / `Forum123`
- `timur_dev` / `Forum123`

В проекте реализованы регистрация с аватаром, вход по логину или email, темы, ответы, пагинация, AJAX-отправка ответов, AJAX-переключение страниц, голоса и Data Seeding.
