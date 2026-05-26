# VPN Manager CLI

Консольный клиент для управления VPN-инфраструктурой.

## Использование

```bash
vpn-cli --<command> [arguments]
```

## Команды

### Аутентификация
| Команда | Описание | Пример |
|---------|----------|--------|
| `--register <login> <pass>` | Регистрация | `--register admin pass123` |
| `--login <login> <pass>` | Войти | `--login admin pass123` |
| `--logout` | Выйти | `--logout` |
| `--whoami` | Текущий пользователь | `--whoami` |

### Заметки
| Команда | Описание | Пример |
|---------|----------|--------|
| `--addNewNote "текст"` | Создать заметку | `--addNewNote "Сменился пароль на шлюзе"` |
| `--listNotes` | Список заметок | `--listNotes` |
| `--deleteNote <id>` | Удалить заметку | `--deleteNote 3` |
| `--editNote <id> "текст"` | Редактировать | `--editNote 3 "Новый текст"` |

### Мониторинг
| Команда | Описание | Пример |
|---------|----------|--------|
| `--stats` | CPU/RAM/HDD | `--stats` |
| `--statsWatch <сек>` | Мониторинг в реальном времени | `--statsWatch 5` |

### Безопасность
| Команда | Описание | Пример |
|---------|----------|--------|
| `--securityLogs` | Логи безопасности | `--securityLogs` |
| `--securityLogs --from 2025-01-01` | Логи за период | `--securityLogs --from 2025-01-01` |

### Администрирование
| Команда | Описание | Пример |
|---------|----------|--------|
| `--adminUsers` | Список пользователей | `--adminUsers` |
| `--adminDeleteNote <uid> <nid>` | Удалить заметку пользователя | `--adminDeleteNote 2 5` |
| `--adminDeleteUser <uid>` | Удалить пользователя | `--adminDeleteUser 3` |
| `--adminCreateAdmin <user>` | Назначить админа | `--adminCreateAdmin bob` |

### Обновления
| Команда | Описание | Пример |
|---------|----------|--------|
| `--checkUpdate` | Проверить обновления | `--checkUpdate` |
| `--applyUpdate` | Применить обновление | `--applyUpdate` |
