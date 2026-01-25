# Документация API системы управления консультациями

## 1. Управление слотами (Slots)
### 1.1. Создание нового слота
**Метод:** `POST`  
**Эндпоинт:** `/api/slots`

**Описание:** Преподаватель создает новый доступный временной слот для консультаций. Система создает ссылку и слот в календаре.

**Запрос:**
```json
{
  "teacher_id": "uuid-t1",
  "start_time": "2026-01-01",
  "end_time": "2026-01-01",
  "meeting_type": "online",
  "description": "Консультация по дипломной работе",
  "metadata": {
    "course_code": "CS101",
    "max_students": 1
  }
}
```

**Параметры:**
- `teacher_id` (string, required) - UUID преподавателя
- `start_time` (timestamp, required) - Начало слота
- `end_time` (timestamp, required) - Окончание слота
- `meeting_type` (enum, required) - Тип встречи: `online` или `offline`
- `description` (string, optional) - Описание слота
- `metadata` (object, optional) - Дополнительные данные

**Успешный ответ (201 Created):**
```json
{
  "id": "slot_abc123",
  "status": "free",
  "start_time": "2026-01-01",
  "end_time": "2026-01-01",
  "public_url": "https://calendar.yandex.ru/event/xyz789",
  "calendar_event_id": "xyz789",
  "created_at": "2025-12-30"
}
```

**Ошибки:**
- `400 Bad Request` - Невалидные данные или конфликт времени
- `401 Unauthorized` - Неавторизованный доступ
- `503 Service Unavailable` - Ошибка интеграции с календарем

---

### 1.2. Получение списка доступных слотов
**Метод:** `GET`  
**Эндпоинт:** `/api/slots/available`

**Описание:** Получение списка свободных слотов для записи студентами. Используется Telegram-ботом для отображения вариантов записи.

**Параметры запроса:**
- `teacher_id` (string, optional) - Фильтр по преподавателю
- `date_from` (timestamp, optional) - Начало периода поиска
- `date_to` (timestamp, optional) - Конец периода поиска

**Успешный ответ (200 OK):**
```json
{
  "slots": [
    {
      "id": "slot_abc123",
      "teacher_id": "uuid-v4",
      "teacher_name": "Иванов И.И.",
      "start_time": "2026-01-15",
      "end_time": "2026-01-15",
      "meeting_type": "online",
      "description": "Консультация по курсу"
    }
  ]
}
```

**Ошибки:**
- `400 Bad Request` - Невалидные параметры запроса
- `500 Internal Server Error` - Ошибка базы данных

---

### 1.3. Удаление слота
**Метод:** `DELETE`  
**Эндпоинт:** `/api/slots/{slot_id}`

**Описание:** Удаление временного слота преподавателем. Автоматически отменяет все связанные бронирования и удаляет событие из календаря.

**Параметры пути:**
- `slot_id` (string, required) - Идентификатор слота

**Запрос:** Тело запроса не требуется

**Успешный ответ (204 No Content):**
Пустое тело ответа

**Ошибки:**
- `404 Not Found` - Слот не найден
- `403 Forbidden` - Пользователь не является владельцем слота
- `503 Service Unavailable` - Ошибка при удалении события из календаря

---

## 2. Управление бронированиями (Bookings)

### 2.1. Создание бронирования
**Метод:** `POST`  
**Эндпоинт:** `/api/bookings`

**Описание:** Студент записывается на выбранный слот через Telegram-бота. Система проверяет доступность, обновляет статус слота и уведомляет преподавателя.

**Запрос:**
```json
{
  "slot_id": "slot_abc123",
  "student_id": "uuid-s1",
  "student_name": "Петров П.П.",
  "topic": "Вопросы по диплому",
  "contact_info": {
    "telegram_id": "@student_username",
    "email": "student@example.com"
  },
  "additional_notes": "Нужна помощь с анализом"
}
```

**Параметры:**
- `slot_id` (string, required) - Идентификатор слота
- `student_id` (string, required) - UUID студента
- `student_name` (string, required) - ФИО студента
- `topic` (string, required) - Тема консультации
- `contact_info` (object, required) - Контактная информация
- `additional_notes` (string, optional) - Дополнительные комментарии

**Успешный ответ (201 Created):**
```json
{
  "booking_id": "book_xyz456",
  "status": "pending",
  "slot_id": "slot_abc123",
  "student_id": "uuid-v4",
  "topic": "Вопросы по диплому",
  "created_at": "2026-01-12",
  "confirmation_deadline": "2026-01-14"
}
```

**Ошибки:**
- `400 Bad Request` - Невалидные данные или обязательные поля отсутствуют
- `409 Conflict` - Слот уже занят или не существует

---

### 2.2. Подтверждение бронирования
**Метод:** `PUT`  
**Эндпоинт:** `/api/bookings/{booking_id}/confirm`

**Описание:** Преподаватель подтверждает запись студента. Система обновляет статус, отправляет уведомление студенту и обновляет событие в календаре.

**Параметры пути:**
- `booking_id` (string, required) - Идентификатор бронирования

**Запрос:**
```json
{
  "teacher_notes": "Подготовьте вопросы по алгоритмам",
  "meeting_link": "https://meet.example.com/room123",
  "send_notification": true
}
```

**Параметры:**
- `teacher_notes` (string, optional) - Комментарии преподавателя
- `meeting_link` (string, optional) - Ссылка на онлайн-встречу
- `send_notification` (boolean, optional, default: true) - Отправлять ли уведомление студенту

**Успешный ответ (200 OK):**
```json
{
  "booking_id": "book_xyz456",
  "status": "confirmed",
  "confirmed_at": "2026-01-13",
  "meeting_link": "https://meet.example.com/room123",
  "notification_sent": true,
  "statistics": {
    "total_bookings": 15,
    "confirmed_this_month": 8
  }
}
```

**Ошибки:**
- `404 Not Found` - Бронирование не найдено
- `400 Bad Request` - Бронирование уже подтверждено или отменено

---

### 2.3. Отмена бронирования
**Метод:** `DELETE`  
**Эндпоинт:** `/api/bookings/{booking_id}`

**Описание:** Отмена бронирования студентом или преподавателем. Освобождает слот для других записей и отправляет уведомления.

**Параметры пути:**
- `booking_id` (string, required) - Идентификатор бронирования

**Запрос:**
```json
{
  "cancelled_by": "student|teacher",
  "reason": "Изменение расписания",
  "notify_other_party": true
}
```

**Параметры:**
- `cancelled_by` (enum, required) - Кто отменяет: `student` или `teacher`
- `reason` (string, optional) - Причина отмены
- `notify_other_party` (boolean, optional, default: true) - Уведомлять вторую сторону

**Успешный ответ (200 OK):**
```json
{
  "booking_id": "book_xyz456",
  "status": "cancelled",
  "cancelled_at": "2026-01-13",
  "cancelled_by": "student",
  "slot_status": "free",
  "notification_sent": true
}
```

**Ошибки:**
- `404 Not Found` - Бронирование не найдено

---

### 2.4. Получение бронирований преподавателя
**Метод:** `GET`  
**Эндпоинт:** `/api/teachers/{teacher_id}/bookings`

**Описание:** Получение списка всех бронирований преподавателя с фильтрацией по статусу.

**Параметры пути:**
- `teacher_id` (string, required) - Идентификатор преподавателя

**Параметры запроса (query parameters):**
- `status` (enum, optional) - Фильтр по статусу: `pending`, `confirmed`, `cancelled`, `completed`
- `date_from` (timestamp, optional) - Начало периода
- `date_to` (timestamp, optional) - Конец периода

**Успешный ответ (200 OK):**
```json
{
  "bookings": [
    {
      "id": "book_xyz456",
      "slot_id": "slot_abc123",
      "student_name": "Петров П.П.",
      "topic": "Вопросы по анализу",
      "status": "confirmed",
      "start_time": "2024-01-15T14:00:00Z",
      "end_time": "2024-01-15T15:00:00Z",
      "created_at": "2024-01-12T11:45:00Z",
      "contact_info": {
        "telegram": "@student_username"
      }
    }
  ],
  "summary": {
    "total": 12,
    "pending": 2,
    "confirmed": 8,
    "cancelled": 2
  }
}
```

**Ошибки:**
- `403 Forbidden` - Запрос данных другого преподавателя
- `404 Not Found` - Преподаватель не найден

---

## 3. Уведомления и напоминания

### 3.1. Отправка напоминания
**Метод:** `POST`  
**Эндпоинт:** `/api/notifications/reminders`

**Описание:** Внутренний API для отправки напоминаний о предстоящих консультациях. Вызывается JournalService.

**Запрос:**
```json
{
  "booking_id": "book_xyz456",
  "recipients": ["student", "teacher"],
  "custom_message": "Не забудьте подготовить материалы"
}
```

**Параметры:**
- `booking_id` (string, required) - Идентификатор бронирования
- `recipients` (array, required) - Получатели: `student`, `teacher`, или оба
- `custom_message` (string, optional) - Кастомное сообщение

**Успешный ответ (200 OK):**
```json
{
  "notification_id": "notif_789",
  "sent_to": {
    "student": true,
    "teacher": true
  },
  "sent_at": "2026-01-15",
  "next_reminder": "2026-01-15"
}
```

**Ошибки:**
- `404 Not Found` - Бронирование не найдено
- `400 Bad Request` - Невалидные параметры получателей

---

### 3.2. Получение истории уведомлений
**Метод:** `GET`  
**Эндпоинт:** `/api/notifications/history`

**Описание:** Получение истории отправленных уведомлений для административных целей.

**Параметры запроса (query parameters):**
- `user_id` (string, optional) - Фильтр по пользователю
- `notification_type` (enum, optional) - Тип уведомления
- `date_from` (timestamp, optional) - Начало периода
- `limit` (integer, optional, default: 50) - Количество записей

**Успешный ответ (200 OK):**
```json
{
  "notifications": [
    {
      "id": "notif_789",
      "type": "reminder",
      "recipient_id": "uuid-s4",
      "recipient_type": "student",
      "message": "Напоминание о консультации через 1 час",
      "sent_at": "2024-01-15",
      "delivery_status": "delivered"
    }
  ],
  "total_count": 150
}
```
