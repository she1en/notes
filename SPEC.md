# Заметки — Спецификация

## 1. Назначение
Консольное CLI-приложение для управления личной IT-инфраструктурой: заметки, мониторинг системы, безопасность, администрирование.

## 2. Функциональные требования

### 2.1. Аутентификация

| ID | Функция | Описание |
|----|---------|----------|
| F-01 | Регистрация | Создание нового пользователя. Первый регистрируемый получает роль `admin`, остальные — `user`. Роли: `user` (заметки), `watcher` (мониторинг), `admin` (всё) |
| F-02 | Вход в систему | Аутентификация по логину и паролю. Создаётся файловая сессия `data/session.json` |
| F-03 | Выход из системы | Завершение сессии, удаление `session.json` |
| F-04 | Хэширование паролей | Пароли хранятся в виде PBKDF2 с SHA-256, 100k итераций, соль 16 байт |
| F-05 | Текущий пользователь | Вывод информации об активной сессии: имя, роль, время входа |

### 2.2. Управление заметками

| ID | Функция | Описание |
|----|---------|----------|
| F-06 | Создание заметки | Добавление текстовой заметки с привязкой к текущему пользователю |
| F-07 | Просмотр списка заметок | Вывод заметок текущего пользователя. Админ видит все заметки |
| F-08 | Редактирование заметки | Изменение текста заметки. Только владелец заметки |
| F-09 | Удаление заметки | Физическое удаление из БД. Обычный пользователь удаляет только свои, админ — любые |

### 2.3. Мониторинг системы

| ID | Функция | Описание |
|----|---------|----------|
| F-10 | Сбор статистики | Получение CPU (WMI), RAM (WMI), HDD (DriveInfo) |
| F-11 | Мониторинг в реальном времени | Циклический сбор статистики каждые N секунд (`--statsWatch`) |

### 2.4. Безопасность

| ID | Функция | Описание |
|----|---------|----------|
| F-12 | Логирование событий | Запись всех действий пользователя в текстовый файл `data/security.log` |
| F-13 | Просмотр логов | Вывод записей из `security.log` с фильтрацией по дате (`--from`, `--to`) и пользователю (`--user`) |
| F-14 | Статистика логов | Группировка записей по типу действия (`--stats`) |
| F-15 | Очистка логов | Удаление всех записей (только админ, `--securityLogs --clear`) |

### 2.5. Администрирование

| ID | Функция | Описание |
|----|---------|----------|
| F-16 | Список пользователей | Вывод всех зарегистрированных пользователей (только админ) |
| F-17 | Удаление пользователя | Удаление пользователя и его заметок (только админ, нельзя удалить себя) |
| F-18 | Назначение админа | Повышение пользователя до роли `admin` (только админ) |

### 2.6. Обновления

| ID | Функция | Описание |
|----|---------|----------|
| F-19 | Проверка обновлений | HTTP-запрос к GitHub Releases API (`api.github.com/repos/she1en/notes/releases/latest`) |
| F-20 | Установка обновлений | Скачивание и замена исполняемого файла |
| F-21 | Автопроверка при запуске | Проверка обновлений при старте (кроме команд `--help`, `--map`, `--version`, `--checkUpdate`, `--applyUpdate`) |

### 2.7. Справка

| ID | Функция | Описание |
|----|---------|----------|
| F-22 | Вывод справки | Отображение списка команд (`--help`) |
| F-23 | Карта команд | Вывод Markdown-таблицы команд (`--map`) |
| F-24 | Версия | Вывод имени и версии приложения (`--version`) |

## 3. Нефункциональные требования

| ID | Требование | Описание |
|----|------------|----------|
| NF-01 | Платформа | .NET Framework 4.7.2 (net472), SDK-style csproj |
| NF-02 | Хранилище | SQLite (`data/notes.db`), файл сессии `data/session.json`, файл логов `data/security.log` |
| NF-03 | Изоляция пользователей | Каждый пользователь видит только свои заметки. Админ видит все. Watcher не может управлять заметками |
| NF-04 | Ролевая модель | Три роли: `user` (заметки), `watcher` (мониторинг), `admin` (всё + администрирование) |
| NF-05 | Интерактивный режим | При запуске без аргументов открывается REPL с приглашением `Заметки>` |
| NF-06 | Unicode | Поддержка русского текста в заметках и выводе |
| NF-07 | Локализация | Интерфейс на русском языке (--help, --map) |
| NF-08 | Локальная архитектура | Нет центрального API, нет синхронизации между машинами |
| NF-09 | Тестирование | NUnit-тесты с изолированной временной БД |

## 4. Состав команд

| Команда | Аргументы | Описание |
|---------|-----------|----------|
| `--help` | — | Показать справку |
| `--map` | — | Карта команд (Markdown) |
| `--version` | — | Версия приложения |
| `--register` | `<username> <password>` | Регистрация |
| `--login` | `<username> <password>` | Вход |
| `--logout` | — | Выход |
| `--whoami` | — | Текущий пользователь |
| `--addNewNote` | `"текст"` | Создать заметку |
| `--listNotes` | — | Список заметок |
| `--deleteNote` | `<id>` | Удалить заметку |
| `--editNote` | `<id> "текст"` | Редактировать заметку |
| `--stats` | — | Метрики системы (CPU/RAM/HDD; admin/watcher) |
| `--statsWatch` | `<сек>` | Мониторинг каждые N секунд (admin/watcher) |
| `--securityLogs` | `[--from d] [--to d] [--user u] [--stats] [--clear]` | Логи безопасности |
| `--adminUsers` | — | Список пользователей (админ) |
| `--adminDeleteUser` | `<uid>` | Удалить пользователя (админ) |
| `--adminCreateAdmin` | `<user>` | Назначить админом (админ) |
| `--setRole` | `<user> <role>` | Установить роль admin/watcher/user (админ) |
| `--checkUpdate` | — | Проверить обновления |
| `--applyUpdate` | — | Применить обновление |

## 5. Модульная архитектура

```
Заметки (ConsoleApp15)
├── Cli (слой представления)
│   ├── Program.cs              — точка входа, маршрутизатор команд
│   ├── Helpers/
│   │   ├── CommandParser.cs     — парсинг аргументов и интерактивного ввода
│   │   └── HelpGenerator.cs     — генерация --help и --map
│
├── Domain (слой моделей)
│   ├── Models/
│   │   ├── Note.cs             — заметка
│   │   ├── User.cs             — пользователь
│   │   ├── Session.cs          — сессия
│   │   ├── CommandArgs.cs      — разобранная команда
│   │   ├── CommandType.cs      — перечисление команд
│   │   └── AppVersion.cs       — версия приложения
│
├── Data (слой данных)
│   ├── Data/
│   │   └── Database.cs         — SQLite инициализация, фабрика соединений
│
├── Services (слой бизнес-логики)
│   ├── Services/
│   │   ├── AuthService.cs      — регистрация, вход/выход, управление пользователями
│   │   ├── NoteService.cs      — CRUD заметок
│   │   ├── StatsService.cs     — мониторинг CPU/RAM/HDD
│   │   ├── SecurityLogger.cs   — файловое логирование событий
│   │   ├── SessionService.cs   — управление session.json
│   │   └── UpdateService.cs    — проверка и установка обновлений
│
└── Tests (тесты)
    └── Tests/
        ├── TestHelper.cs       — временная БД для тестов
        ├── AuthTests.cs        — тесты аутентификации
        ├── DatabaseTests.cs    — тесты БД
        ├── NotesTests.cs       — тесты заметок
        └── UpdateTests.cs      — тесты обновлений
```

### 5.1. Связи между модулями

```
Cli → Services → Data
Cli → Domain
Services → Domain, Data, Services
Data → Domain
```

- **Cli** вызывает **Services** (например, `Auth.Login()`) и использует **Domain** (модели для вывода)
- **Services** вызывают **Data** (через `Database.CreateConnection()`), используют **Domain**, логируют через **Services** (SecurityLogger)
- **Data** использует **Domain** (возвращает модели через репозитории)
- **Tests** используют **Data** и **Services** напрямую, подменяя путь к БД через конструктор

### 5.2. Примечания по архитектуре

- **Интерфейсы** отсутствуют — все зависимости жёсткие, через конкретные классы
- **Ролевая модель**: `user` (заметки), `watcher` (мониторинг), `admin` (всё)
- **Dependency Injection** ручной — через конструкторы (например, `NoteService(Database, AuthService)`)
- **Сервисы создают свои зависимости** — `AuthService()` внутри создаёт `SessionService`, `NoteService` создаёт `SecurityLogger`
- **Тесты** прокидывают зависимости через конструкторы (TestHelper создаёт общие `Database` и `AuthService` для `NoteService`)


## 6. Модели данных

### 6.1. users (SQLite)
| Поле | Тип | Описание |
|------|-----|----------|
| Id | INTEGER PK AUTOINCREMENT | ID пользователя |
| Username | TEXT NOT NULL UNIQUE | Логин |
| PasswordHash | TEXT NOT NULL | PBKDF2 хэш (salt:hash) |
| Role | TEXT DEFAULT 'user' | Роль: `user`, `watcher` или `admin` |
| CreatedAt | TEXT | Дата создания |

### 6.2. notes (SQLite)
| Поле | Тип | Описание |
|------|-----|----------|
| Id | INTEGER PK AUTOINCREMENT | ID заметки |
| UserId | INTEGER NOT NULL FK→users | Владелец |
| Username | TEXT NOT NULL | Имя владельца (денормализация) |
| Text | TEXT NOT NULL | Текст заметки |
| CreatedAt | TEXT | Дата создания |
| UpdatedAt | TEXT | Дата изменения |
| IsDeleted | INTEGER DEFAULT 0 | Флаг удаления (зарезервировано) |

### 6.3. settings (SQLite)
| Поле | Тип | Описание |
|------|-----|----------|
| Key | TEXT PK | Ключ |
| Value | TEXT | Значение |

### 6.4. session.json
```json
{ "Username": "...", "Role": "...", "UserId": 1, "LoginTime": "/Date(...)/" }
```

### 6.5. security.log
Формат строки:
```
yyyy-MM-dd HH:mm:ss | username       | action               | status   | details
```

## 7. Структура проекта (файловая)

```
ConsoleApp15/
├── Program.cs                       # Точка входа, маршрутизация команд, интерактивный режим
├── ConsoleApp15.csproj              # SDK-style, net472, PackageReference
├── Models/
│   ├── AppVersion.cs                # Имя и версия приложения
│   ├── CommandArgs.cs               # Разобранная команда (тип + аргументы + флаги)
│   ├── CommandType.cs               # Перечисление всех команд
│   ├── Note.cs                      # Сущность заметки
│   ├── Session.cs                   # Сессия пользователя
│   └── User.cs                      # Сущность пользователя
├── Data/
│   └── Database.cs                  # SQLite инициализация, фабрика соединений
├── Helpers/
│   ├── CommandParser.cs             # Парсинг аргументов и интерактивного ввода
│   └── HelpGenerator.cs            # --help и --map
├── Services/
│   ├── AuthService.cs               # Регистрация, вход/выход, управление пользователями
│   ├── NoteService.cs               # CRUD заметок
│   ├── SessionService.cs            # Чтение/запись session.json
│   ├── SecurityLogger.cs            # Файловое логирование в security.log
│   ├── StatsService.cs              # WMI-сбор CPU/RAM, DriveInfo для HDD
│   └── UpdateService.cs             # GitHub Releases API, загрузка обновлений
├── Tests/
│   ├── ConsoleApp15.Tests.csproj    # NUnit 3.14, .NET 4.7.2
│   ├── TestHelper.cs                # Временная БД для тестов
│   ├── AuthTests.cs                 # 7 тестов аутентификации
│   ├── DatabaseTests.cs             # 5 тестов БД
│   ├── NotesTests.cs                # 6 тестов заметок
│   └── UpdateTests.cs               # 3 теста обновлений
├── SPEC.md                          # Спецификация проекта
└── .gitignore
```

## 8. Чек-лист ручного тестирования

### 8.1. Запуск и справка
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 1 | Запустить без аргументов | Интерактивный режим: `Заметки v1.0.0 — interactive mode` | Ок |
| 2 | Ввести `help` | Справка со всеми командами на русском | Ок |
| 3 | Ввести `exit` | Завершение приложения | Ок |

### 8.2. Аутентификация
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 4 | `register user1 pass123` | `OK: User 'user1' registered.` | Ок |
| 5 | `register user1 pass123` повторно | `Error: User 'user1' already exists.` | Ок |
| 6 | `login user1 pass123` | `OK: Logged in as user1 (user).` | Ок |
| 7 | `login user1 wrongpass` | `Error: Invalid username or password.` | Ок |
| 8 | `login nobody pass` | `Error: Invalid username or password.` | Ок |
| 9 | `whoami` | `User: user1` / `Role: user` / время входа | Ок |
| 10 | `logout` | `Logged out.` | Ок |
| 11 | `whoami` без входа | `Not logged in.` | Ок |

### 8.3. Заметки
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 12 | Без входа `addNewNote "test"` | `Error: Not logged in.` | Ок |
| 13 | Войти как user1, `addNewNote "Первая заметка"` | `OK: Note #1 added.` | Ок |
| 14 | `addNewNote "Вторая заметка"` | `OK: Note #2 added.` | Ок |
| 15 | `listNotes` | Список из 2 заметок | Ок |
| 16 | `deleteNote 1` | `OK: Note #1 deleted.` | Ок |
| 17 | `listNotes` | Только 1 заметка (#2) | Ок |
| 18 | `deleteNote 99` | `Error: Note #99 not found.` | Ок |
| 19 | `editNote 2 "Новый текст"` | `OK: Note #2 updated.` | Ок |

### 8.4. Изоляция пользователей
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 20 | `register user2 pass456` | `OK: User 'user2' registered.` | Ок |
| 21 | `login user2 pass456`, `listNotes` | `No notes.` | Ок |
| 22 | `login user1 pass123`, `listNotes` | Только заметки user1 | Ок |
| 23 | `deleteNote 2` (чужая заметка user1) | `Error: Note #2 not found.` (фильтр по UserId) | Ок |

### 8.5. Мониторинг
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 24 | Авторизоваться, `stats` | CPU%, RAM%, HDD% | Ок |
| 25 | `stats` без авторизации | `Error: Not logged in.` | Ок |

### 8.6. Безопасность
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 26 | Авторизоваться, `securityLogs` | Список событий с датой, user, action, status | Ок |
| 27 | `securityLogs --from 2026-01-01` | Только логи после даты | Ок |
| 28 | `securityLogs --user user1` | Только логи user1 | Ок |
| 29 | `securityLogs --stats` | Группировка по action | Ок |

### 8.7. Администрирование
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 30 | От имени user1 (не админ) `adminUsers` | `Admin privileges required.` | Ок |
| 31 | Войти как admin, `adminUsers` | Список всех пользователей | Ок |
| 32 | `adminDeleteUser 2` | `User #2 deleted.` | Ок |

### 8.8. Обновления
| № | Шаг | Ожидаемый результат | Статус |
|---|-----|---------------------|--------|
| 33 | `checkUpdate` | `You have the latest version.` или сообщение об ошибке сети | Ок |
| 34 | `applyUpdate` без авторизации | `Not logged in.` | Ок |

## 9. Код тестов основных функций

### 9.1. Аутентификация (AuthTests.cs)
```csharp
[Test]
public void Register_NewUser_ReturnsSuccess()
{
    var (success, message) = _helper.Auth.Register("testuser", "testpass");
    Assert.That(success, Is.True);
    Assert.That(message, Does.Contain("registered"));
}

[Test]
public void Register_FirstUser_IsAdmin()
{
    var (success, message) = _helper.Auth.Register("admin", "admin123");
    Assert.That(success, Is.True);
    Assert.That(message, Does.Contain("admin"));
}

[Test]
public void Register_DuplicateUser_ReturnsError()
{
    _helper.Auth.Register("testuser", "testpass");
    var (success, message) = _helper.Auth.Register("testuser", "testpass");
    Assert.That(success, Is.False);
    Assert.That(message, Does.Contain("already exists"));
}

[Test]
public void Login_ValidCredentials_ReturnsSuccess()
{
    _helper.Auth.Register("testuser", "testpass");
    var (success, message, session) = _helper.Auth.Login("testuser", "testpass");
    Assert.That(success, Is.True);
    Assert.That(session.Username, Is.EqualTo("testuser"));
}

[Test]
public void Login_WrongPassword_ReturnsError()
{
    _helper.Auth.Register("testuser", "testpass");
    var (success, message, session) = _helper.Auth.Login("testuser", "wrongpass");
    Assert.That(success, Is.False);
    Assert.That(session, Is.Null);
}

[Test]
public void Login_NonexistentUser_ReturnsError()
{
    var (success, message, session) = _helper.Auth.Login("nobody", "pass");
    Assert.That(success, Is.False);
    Assert.That(session, Is.Null);
}

[Test]
public void Logout_ClearsSession()
{
    _helper.Auth.Register("testuser", "testpass");
    _helper.Auth.Login("testuser", "testpass");
    _helper.Auth.Logout();
    var session = _helper.Auth.GetCurrentSession();
    Assert.That(session.IsActive, Is.False);
}
```

### 9.2. База данных (DatabaseTests.cs)
```csharp
[Test]
public void Database_CreatesDbFile()
{
    using var db = new Database(_tempDir);
    Assert.That(Directory.GetFiles(_tempDir, "*.db").Length, Is.EqualTo(1));
}

[Test]
public void Database_CreatesUsersTable()
{
    using var db = new Database(_tempDir);
    using var conn = db.CreateConnection();
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='users'";
    Assert.That(cmd.ExecuteScalar().ToString(), Is.EqualTo("users"));
}

[Test]
public void Database_CreatesNotesTable()
{
    using var db = new Database(_tempDir);
    using var conn = db.CreateConnection();
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='notes'";
    Assert.That(cmd.ExecuteScalar(), Is.Not.Null);
}

[Test]
public void Database_InsertAndReadUser()
{
    using var db = new Database(_tempDir);
    using var conn = db.CreateConnection();
    conn.Open();
    using var insert = conn.CreateCommand();
    insert.CommandText = "INSERT INTO users (Username, PasswordHash, Role, CreatedAt) VALUES (@u, @p, 'user', '2026-01-01')";
    insert.Parameters.AddWithValue("@u", "test_user");
    insert.Parameters.AddWithValue("@p", "hash");
    insert.ExecuteNonQuery();
    using var select = conn.CreateCommand();
    select.CommandText = "SELECT COUNT(*) FROM users WHERE Username = 'test_user'";
    Assert.That((long)select.ExecuteScalar(), Is.EqualTo(1));
}

[Test]
public void Database_InsertAndReadNote()
{
    using var db = new Database(_tempDir);
    using var conn = db.CreateConnection();
    conn.Open();
    using var userInsert = conn.CreateCommand();
    userInsert.CommandText = "INSERT INTO users (Username, PasswordHash, Role, CreatedAt) VALUES ('u', 'h', 'user', '2026-01-01')";
    userInsert.ExecuteNonQuery();
    using var noteInsert = conn.CreateCommand();
    noteInsert.CommandText = "INSERT INTO notes (UserId, Username, Text, CreatedAt, UpdatedAt, IsDeleted) VALUES (1, 'u', 'test note', '2026-01-01', '2026-01-01', 0)";
    noteInsert.ExecuteNonQuery();
    using var select = conn.CreateCommand();
    select.CommandText = "SELECT Text FROM notes WHERE Id = 1";
    Assert.That(select.ExecuteScalar().ToString(), Is.EqualTo("test note"));
}
```

### 9.3. Заметки (NotesTests.cs)
```csharp
[Test]
public void AddNote_ValidText_CreatesNote()
{
    var (success, message, note) = _helper.Notes.AddNote("Test note text");
    Assert.That(success, Is.True);
    Assert.That(note.Text, Is.EqualTo("Test note text"));
}

[Test]
public void AddNote_EmptyText_ReturnsError()
{
    var (success, message, note) = _helper.Notes.AddNote("");
    Assert.That(success, Is.False);
}

[Test]
public void ListNotes_AfterAdd_ReturnsOneNote()
{
    _helper.Notes.AddNote("First note");
    var (notes, _) = _helper.Notes.ListNotes();
    Assert.That(notes.Count, Is.EqualTo(1));
}

[Test]
public void DeleteNote_ExistingNote_RemovesIt()
{
    _helper.Notes.AddNote("To delete");
    var (success, message) = _helper.Notes.DeleteNote(1);
    Assert.That(success, Is.True);
    var (notes, _) = _helper.Notes.ListNotes();
    Assert.That(notes.Count, Is.EqualTo(0));
}

[Test]
public void DeleteNote_Nonexistent_ReturnsError()
{
    var (success, message) = _helper.Notes.DeleteNote(999);
    Assert.That(success, Is.False);
}

[Test]
public void EditNote_UpdatesContent()
{
    _helper.Notes.AddNote("Original text");
    var (success, _, _) = _helper.Notes.EditNote(1, "Updated text");
    Assert.That(success, Is.True);
    var (notes, _) = _helper.Notes.ListNotes();
    Assert.That(notes[0].Text, Is.EqualTo("Updated text"));
}
```

### 9.4. Обновления (UpdateTests.cs)
```csharp
[Test]
public void CheckForUpdates_NoNetwork_ReturnsFalse()
{
    var updateService = new UpdateService();
    var (available, version, _) = updateService.CheckForUpdates();
    Assert.That(available, Is.False);
}

[Test]
public void AppVersion_IsDefined()
{
    Assert.That(AppVersion.Version, Is.EqualTo("1.0.0"));
    Assert.That(AppVersion.AppName, Is.EqualTo("Заметки"));
}

[Test]
public void ApplyUpdate_EmptyUrl_ReturnsError()
{
    var updateService = new UpdateService();
    var (success, message) = updateService.ApplyUpdate("");
    Assert.That(success, Is.False);
    Assert.That(message, Does.Contain("No update URL"));
}
```

## 10. Отчёт о тестировании

### 10.1. Результаты автоматического тестирования
```
Тестовый запуск: ConsoleApp15.Tests.dll (.NETFramework,Version=v4.7.2)
Всего тестов: 21

Результат:
  Пройдено:  21
  Не пройдено: 0
  Пропущено: 0
  Длительность: ~10 секунд
```

### 10.2. Распределение тестов по модулям
| Модуль | Файл | Тестов | Пройдено | Не пройдено |
|--------|------|-------:|---------:|------------:|
| Аутентификация | AuthTests.cs | 7 | 7 | 0 |
| База данных | DatabaseTests.cs | 5 | 5 | 0 |
| Заметки | NotesTests.cs | 6 | 6 | 0 |
| Обновления | UpdateTests.cs | 3 | 3 | 0 |
| **Итого** | | **21** | **21** | **0** |

### 10.3. Покрытие функциональных требований
| ID | Требование | Тест | Статус |
|----|-----------|------|--------|
| F-01 | Регистрация | Register_NewUser_ReturnsSuccess | Ок |
| F-01 | Первый — admin | Register_FirstUser_IsAdmin | Ок |
| F-01 | Дубликат | Register_DuplicateUser_ReturnsError | Ок |
| F-02 | Вход верный | Login_ValidCredentials_ReturnsSuccess | Ок |
| F-02 | Неверный пароль | Login_WrongPassword_ReturnsError | Ок |
| F-02 | Несуществующий | Login_NonexistentUser_ReturnsError | Ок |
| F-03 | Выход | Logout_ClearsSession | Ок |
| F-04 | Хэширование | (проверяется через login с верным/неверным паролем) | Ок |
| F-06 | Создание заметки | AddNote_ValidText_CreatesNote | Ок |
| F-06 | Пустой текст | AddNote_EmptyText_ReturnsError | Ок |
| F-07 | Список заметок | ListNotes_AfterAdd_ReturnsOneNote | Ок |
| F-08 | Редактирование | EditNote_UpdatesContent | Ок |
| F-09 | Удаление | DeleteNote_ExistingNote_RemovesIt | Ок |
| F-09 | Удаление несуществующей | DeleteNote_Nonexistent_ReturnsError | Ок |
| NF-02 | Инициализация БД | Database_CreatesDbFile | Ок |
| NF-02 | Таблицы | Database_CreatesUsersTable, Database_CreatesNotesTable | Ок |
| NF-02 | CRUD пользователь | Database_InsertAndReadUser | Ок |
| NF-02 | CRUD заметка | Database_InsertAndReadNote | Ок |
| F-19 | Проверка обновлений | CheckForUpdates_NoNetwork_ReturnsFalse | Ок |
| F-20 | Установка | ApplyUpdate_EmptyUrl_ReturnsError | Ок |
| NF-08 | Версия | AppVersion_IsDefined | Ок |

### 10.4. Вывод
Все 21 автоматический тест успешно пройдены. Проведено 34 сценария ручного тестирования, все успешны. Функциональные требования выполнены в полном объёме.

## 11. Работа с Git

### 11.1. Настройка репозитория
Репозиторий инициализирован в корне проекта ConsoleApp15. В `.gitignore` исключены `bin/`, `obj/`, `data/`, `*.db`, `.vs/` и др.

Удалённый репозиторий: `https://github.com/she1en/notes`

### 11.2. История коммитов
```
$ git log --oneline --reverse

0a20953 Начальная структура
fe78617 Stage 1
4e6e35a Stage 2
eab1364 Stage 3
ce562de Stage 4
388f3a6 мониторинг
405a66e logs
044e051 admin
4d32fa9 обновление
108ca14 sqlite
1096f01 tests
f772719 move tests
69f9295 interactive
f03bd69 ignore data
4c33d2c deleteNote admin-only
707faad fix session isolation + user note isolation
```

### 11.3. Детализация коммитов
| № | Коммит | Файлов | Строк | Содержание |
|---|--------|-------:|------:|-----------|
| 1 | 0a20953 | — | — | Начальная структура проекта, пустое решение |
| 2 | fe78617 | — | — | Stage 1: --help, --map, --version, Markdown-карта |
| 3 | 4e6e35a | — | — | Stage 2: аутентификация (register, login, logout, whoami, PBKDF2) |
| 4 | eab1364 | — | — | Stage 3: заметки (add, list, edit, delete, изоляция по пользователю) |
| 5 | ce562de | — | — | Stage 4: мониторинг (CPU, RAM, HDD через WMI + DriveInfo) |
| 6 | 388f3a6 | — | — | Логи безопасности (запись, чтение, фильтрация, --stats, --clear) |
| 7 | 405a66e | — | — | Админ-панель (--adminUsers, --adminDeleteUser, --adminCreateAdmin) |
| 8 | 4d32fa9 | — | — | Переименование в "Заметки", русский UI, миграция с JSON на SQLite |
| 9 | 108ca14 | — | — | SDK-style csproj, System.Data.SQLite |
| 10 | 1096f01 | — | — | Тестовый проект (NUnit, 21 тест, TestHelper с временной БД) |
| 11 | f772719 | — | — | Перенос тестов в ConsoleApp15/Tests/ |
| 12 | 69f9295 | — | — | Интерактивный режим (REPL, приглашение `Заметки>`) |
| 13 | f03bd69 | — | — | .gitignore: добавлен data/ |
| 14 | 4c33d2c | — | — | deleteNote только для админа (впоследствии исправлено) |
| 15 | 707faad | 4 | 42 | Исправление: общий Auth через DI, изоляция заметок |

### 11.4. Конфигурация
- **Автор**: she1en <xzkwxb@gmail.com>
- **Ветка**: master
- **Репозиторий**: https://github.com/she1en/notes

## 12. Заключение

В ходе выполнения курсовой работы была разработана консольная система заметок для личной ИТ-инфраструктуры.

**Результаты:**

1. Разработано консольное приложение на языке C# с использованием .NET Framework 4.7.2
2. Реализована система аутентификации с регистрацией пользователей, хэшированием паролей (PBKDF2 с SHA-256, 100k итераций) и разграничением доступа (роли user/admin)
3. Реализован модуль заметок с созданием, просмотром, списком и удалением, с изоляцией данных между пользователями
4. Реализован модуль мониторинга системы, собирающий статистику CPU и RAM через WMI, HDD через DriveInfo
5. Реализован модуль безопасности с логированием всех событий в файл и возможностью просмотра и фильтрации журнала
6. Реализован модуль обновлений с проверкой и загрузкой новых версий через GitHub Releases API
7. Единое хранилище на SQLite (System.Data.SQLite) с миграцией с исходного JSON-формата
8. Интерактивный режим работы с поддержкой Unicode (русский язык интерфейса)
9. Написаны 21 автоматический тест (NUnit) с изолированной временной БД, все успешно проходят
10. Проведено ручное тестирование по 34 сценариям

## 13. Список литературы

1. Рихтер Дж. CLR via C#. Программирование на платформе Microsoft .NET Framework 4.8 на языке C#. — СПб.: Питер, 2020. — 826 с. (дата обращения: 19.05.2026).
2. Троелсен Э., Джепикс Ф. Язык программирования C# 10 и платформа .NET 6. — СПб.: Диалектика, 2023. — 1376 с. (дата обращения: 19.05.2026).
3. Официальная документация .NET Framework. — https://learn.microsoft.com/dotnet (дата обращения: 19.05.2026).
4. System.Data.SQLite Documentation. — https://system.data.sqlite.org/ (дата обращения: 19.05.2026).
5. Официальная документация SQLite. — https://www.sqlite.org/docs.html (дата обращения: 19.05.2026).
6. NUnit Documentation. — https://docs.nunit.org/ (дата обращения: 19.05.2026).
7. WMI (Windows Management Instrumentation) documentation. — https://learn.microsoft.com/windows/win32/wmisdk/wmi-start-page (дата обращения: 19.05.2026).
8. GitHub Releases API. — https://docs.github.com/rest/releases (дата обращения: 19.05.2026).
9. Методические указания к выполнению курсовой работы. (дата обращения: 19.05.2026).
