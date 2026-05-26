using System;
using ConsoleApp15.Models;

namespace ConsoleApp15.Helpers
{
    public static class HelpGenerator
    {
        public static void ShowHelp()
        {
            Console.WriteLine($"=== {AppVersion.AppName} v{AppVersion.Version} ===");
            Console.WriteLine("Usage: заметки --<command> [arguments]");
            Console.WriteLine();
            Console.WriteLine("Команды:");
            Console.WriteLine("  --help                         Показать справку");
            Console.WriteLine("  --map                          Карта команд (Markdown)");
            Console.WriteLine("  --version                      Версия");
            Console.WriteLine("  --register <login> <pass>      Регистрация");
            Console.WriteLine("  --login <login> <pass>         Войти");
            Console.WriteLine("  --logout                       Выйти");
            Console.WriteLine("  --whoami                       Текущий пользователь");
            Console.WriteLine("  --addNewNote \"текст\"          Создать заметку");
            Console.WriteLine("  --listNotes                    Список заметок");
            Console.WriteLine("  --deleteNote <id>              Удалить заметку");
            Console.WriteLine("  --editNote <id> \"текст\"       Редактировать заметку");
            Console.WriteLine("  --stats                        Метрики системы");
            Console.WriteLine("  --statsWatch <сек>             Мониторинг каждые N сек");
            Console.WriteLine("  --securityLogs                 Логи безопасности");
            Console.WriteLine("  --adminUsers                   Список пользователей (админ)");
            Console.WriteLine("  --adminDeleteNote <uid> <nid>  Удалить заметку (админ)");
            Console.WriteLine("  --adminDeleteUser <uid>        Удалить пользователя (админ)");
            Console.WriteLine("  --adminCreateAdmin <user>      Назначить админа (админ)");
            Console.WriteLine("  --checkUpdate                  Проверить обновления");
            Console.WriteLine("  --applyUpdate                  Применить обновление");
        }

        public static void ShowMap()
        {
            Console.WriteLine($"# {AppVersion.AppName} — Карта команд");
            Console.WriteLine();
            Console.WriteLine("## Аутентификация");
            Console.WriteLine("| Команда | Описание | Пример |");
            Console.WriteLine("|---------|----------|--------|");
            Console.WriteLine("| `--register <login> <pass>` | Регистрация | `--register john pass123` |");
            Console.WriteLine("| `--login <login> <pass>` | Вход | `--login john pass123` |");
            Console.WriteLine("| `--logout` | Выход | `--logout` |");
            Console.WriteLine("| `--whoami` | Текущий пользователь | `--whoami` |");
            Console.WriteLine();
            Console.WriteLine("## Заметки");
            Console.WriteLine("| Команда | Описание | Пример |");
            Console.WriteLine("|---------|----------|--------|");
            Console.WriteLine("| `--addNewNote \"текст\"` | Создать заметку | `--addNewNote \"Новая заметка\"` |");
            Console.WriteLine("| `--listNotes` | Список заметок | `--listNotes` |");
            Console.WriteLine("| `--deleteNote <id>` | Удалить заметку | `--deleteNote 3` |");
            Console.WriteLine("| `--editNote <id> \"текст\"` | Редактировать | `--editNote 3 \"Новый текст\"` |");
            Console.WriteLine();
            Console.WriteLine("## Мониторинг");
            Console.WriteLine("| Команда | Описание | Пример |");
            Console.WriteLine("|---------|----------|--------|");
            Console.WriteLine("| `--stats` | CPU/RAM/HDD | `--stats` |");
            Console.WriteLine("| `--statsWatch <сек>` | Мониторинг в реальном времени | `--statsWatch 5` |");
            Console.WriteLine();
            Console.WriteLine("## Безопасность");
            Console.WriteLine("| Команда | Описание | Пример |");
            Console.WriteLine("|---------|----------|--------|");
            Console.WriteLine("| `--securityLogs` | Логи безопасности | `--securityLogs` |");
            Console.WriteLine("| `--securityLogs --from 2025-01-01 --to 2025-12-31` | Логи с фильтром | `--securityLogs --from 2025-01-01` |");
            Console.WriteLine();
            Console.WriteLine("## Администрирование");
            Console.WriteLine("| Команда | Описание | Пример |");
            Console.WriteLine("|---------|----------|--------|");
            Console.WriteLine("| `--adminUsers` | Список пользователей | `--adminUsers` |");
            Console.WriteLine("| `--adminDeleteNote <uid> <nid>` | Удалить заметку пользователя | `--adminDeleteNote 2 5` |");
            Console.WriteLine("| `--adminDeleteUser <uid>` | Удалить пользователя | `--adminDeleteUser 3` |");
            Console.WriteLine("| `--adminCreateAdmin <user>` | Назначить админом | `--adminCreateAdmin bob` |");
            Console.WriteLine();
            Console.WriteLine("## Обновления");
            Console.WriteLine("| Команда | Описание | Пример |");
            Console.WriteLine("|---------|----------|--------|");
            Console.WriteLine("| `--checkUpdate` | Проверить обновления | `--checkUpdate` |");
            Console.WriteLine("| `--applyUpdate` | Применить обновление | `--applyUpdate` |");
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine($"*{AppVersion.AppName} v{AppVersion.Version}*");
        }
    }
}
