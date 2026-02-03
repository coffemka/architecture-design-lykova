# Итоговая коллекция
<img width="697" height="770" alt="image" src="https://github.com/user-attachments/assets/e5652823-fc23-4add-b57a-17b84bd76ea8" />

# Тестирование API
## 1. Тестирование Slots API
### 1.1. Создание нового слота POST {{baseUrl}} /api/slots

**Тест 1: 201**

<img width="1249" height="638" alt="image" src="https://github.com/user-attachments/assets/e80c32e6-7a60-428e-ac32-542f29465082" />

**Ответ**

<img width="1243" height="404" alt="image" src="https://github.com/user-attachments/assets/5c97360a-e0ee-4498-b64f-58d776f3062a" />

**Код автотестов**

```
pm.test("Статус ответа 201 Created", function() {
    pm.response.to.have.status(201);
});

pm.test("Ответ содержит все обязательные поля", function() {
    const response = pm.response.json();
    
    pm.expect(response).to.have.property("id");
    pm.expect(response).to.have.property("status");
    pm.expect(response).to.have.property("startTime");
    pm.expect(response).to.have.property("endTime");
    pm.expect(response).to.have.property("publicUrl");
    pm.expect(response).to.have.property("calendarEventId");
    pm.expect(response).to.have.property("createdAt");
});
```

**Результат автотестов**

<img width="1270" height="415" alt="image" src="https://github.com/user-attachments/assets/e343ceae-414a-416c-a55b-d4c36d74e2d3" />

---

**Тест 2: 400**

<img width="1266" height="526" alt="image" src="https://github.com/user-attachments/assets/29131ec2-5cd4-4105-9ee1-51428ad80b10" />

**Ответ**

<img width="1244" height="532" alt="image" src="https://github.com/user-attachments/assets/bd8abc68-4181-4f91-a9a7-807355ed8bc7" />


**Код автотестов**

```
pm.test("Статус ответа 400 Created", function() {
    pm.response.to.have.status(400);
});
```

**Результат автотестов**

<img width="534" height="190" alt="image" src="https://github.com/user-attachments/assets/f098fcff-e899-424d-a755-0fd850ea7f25" />

### 1.2. Получение доступных слотов GET  /api/slots/available

**Тест 1: 200**

<img width="1261" height="837" alt="image" src="https://github.com/user-attachments/assets/8b791ce2-8fbc-4b40-83ed-a4511e648279" />

**Ответ**

<img width="1241" height="498" alt="image" src="https://github.com/user-attachments/assets/fcd3fa47-ea2e-4cff-aba7-0ef82d75faaa" />

**Код автотестов**

```
pm.test("Код статуса 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Есть все необходимые данные", function () {
    const response = pm.response.json();
    if (response.slots.length > 0) {
        const slot = response.slots[0];
        pm.expect(slot).to.have.property('id');
        pm.expect(slot).to.have.property('teacherId');
        pm.expect(slot).to.have.property('startTime');
        pm.expect(slot).to.have.property('endTime');
    }
});
```

**Результат автотестов**

<img width="454" height="163" alt="image" src="https://github.com/user-attachments/assets/e66359df-c69f-4c81-8a38-a1ec48811269" />


---

**Тест 2: 400 - неправильные данные**

<img width="1251" height="470" alt="image" src="https://github.com/user-attachments/assets/8b09a08b-5658-43be-908e-6cb50caeca91" />


**Ответ**

<img width="586" height="118" alt="image" src="https://github.com/user-attachments/assets/2b598b73-4211-45eb-ac6e-6370bb3ddedd" />


**Код автотестов**

```
pm.test("Статус 400 Bad Request при невалидных данных", function() {
    pm.response.to.have.status(400);
});
```

**Результат автотестов**

<img width="586" height="118" alt="image" src="https://github.com/user-attachments/assets/8c146eb1-7d97-4a99-9aee-f2a7c44a017e" />

### 1.3. Удаление слота DELETE  /api/slots/{slotId}

**Тест 1: 204**

<img width="1286" height="411" alt="image" src="https://github.com/user-attachments/assets/3ee054a3-5029-4e7c-bbd1-1816d606dd5a" />

**Ответ**

<img width="1255" height="359" alt="image" src="https://github.com/user-attachments/assets/da0a634d-1272-4262-8a57-5df449675bff" />


**Код автотестов**

```
pm.test("Статус 204 No Content", () => {
    pm.response.to.have.status(204);
});

pm.test("Тело ответа пустое", () => {
    pm.response.to.have.body("");
});

console.log(`Слот ${pm.environment.get("slotId")} удален`);
```

**Результат автотестов**

<img width="502" height="260" alt="image" src="https://github.com/user-attachments/assets/8cc6b69f-87d9-4b4b-a856-859d6afbbadc" />


---

**Тест 2: 404 - слот не найден**

<img width="1289" height="611" alt="image" src="https://github.com/user-attachments/assets/036d48ec-38d9-4523-bdf3-de4fb33d7aef" />


**Ответ**

<img width="1281" height="235" alt="image" src="https://github.com/user-attachments/assets/982d899e-5214-47b2-8ffc-c8a0e6c5f22a" />



**Код автотестов**

```
pm.test("Статус 404 Not Found", () => {
    pm.response.to.have.status(404);
});
```

**Результат автотестов**

<img width="385" height="110" alt="image" src="https://github.com/user-attachments/assets/ceb6ddde-f14d-4b78-acd5-ff593870e2f1" />

## 2. Тестирование Booking API
### 2.1. Создание нового бронирования POST {{baseUrl}}/api/bookings

**Тест 1: 201**

<img width="1333" height="782" alt="image" src="https://github.com/user-attachments/assets/e62b037a-4d66-4a05-9452-0ebf99d9c290" />

**Ответ**

<img width="1288" height="480" alt="image" src="https://github.com/user-attachments/assets/7ef13b43-6c52-41cb-89d1-5b569216b6cc" />

**Код автотестов**

```
pm.test("Статус 201 создан", function () {
    pm.response.to.have.status(201);
});

pm.test("Правильная структура", function () {
    const response = pm.response.json();
    pm.expect(response).to.have.property('bookingId');
    pm.expect(response).to.have.property('status', 0);
    pm.expect(response).to.have.property('confirmationDeadline');
});
```

**Результат автотестов**

<img width="406" height="161" alt="image" src="https://github.com/user-attachments/assets/282ebbc9-dfcf-47b9-8a07-3b10a760beca" />

**Тест 2: 409**

<img width="1281" height="568" alt="image" src="https://github.com/user-attachments/assets/4f9491f4-17cb-4373-bdda-7524e3692ea8" />

**Ответ**

<img width="1274" height="175" alt="image" src="https://github.com/user-attachments/assets/00634abd-e474-45e3-bd68-1a912a422849" />


**Код автотестов**

```
pm.test("Статус 409 создан", function () {
    pm.response.to.have.status(409);
});
```

**Результат автотестов**

<img width="410" height="106" alt="image" src="https://github.com/user-attachments/assets/9ab8fab9-d9e9-4d1d-b610-5ed0f0a3c92e" />

### 2.2. Подтверждение бронирования PUT /api/bookings/{bookingId}/confirm

**Тест 1: 200**

<img width="1259" height="399" alt="image" src="https://github.com/user-attachments/assets/e896d4e7-c190-487c-b829-a9abedf00a12" />
<img width="1064" height="378" alt="image" src="https://github.com/user-attachments/assets/48feca9c-b3e7-4050-9699-844eabf55b1d" />

**Ответ**

<img width="1262" height="411" alt="image" src="https://github.com/user-attachments/assets/15420683-1c95-4792-a765-4b6842a3b573" />


**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Ответ содержит все обязательные поля", function() {
    const response = pm.response.json();
    
    pm.expect(response).to.have.property("bookingId");
    pm.expect(response).to.have.property("status");
    pm.expect(response).to.have.property("confirmedAt");
    pm.expect(response).to.have.property("meetingLink");
    pm.expect(response).to.have.property("notificationSent");
    pm.expect(response).to.have.property("statistics");
});

```

**Результат автотестов**

<img width="576" height="165" alt="image" src="https://github.com/user-attachments/assets/ba62aca7-4f3f-48c9-8a5f-88f1116ce474" />

**Тест 2: 404**
<img width="1286" height="462" alt="image" src="https://github.com/user-attachments/assets/31bdebc8-099b-49e7-bb93-88a3faaf0280" />
<img width="1075" height="394" alt="image" src="https://github.com/user-attachments/assets/4946790f-9d88-46b9-90c0-9d0e2520ec79" />

**Ответ**
<img width="1275" height="223" alt="image" src="https://github.com/user-attachments/assets/fcd8bf7a-9599-48fc-af29-e96083190f36" />

**Код автотестов**

```
pm.test("Статус ответа 404 Not Found", function() {
    pm.response.to.have.status(404);
});

pm.test("Сообщение об ошибке", function() {
    const responseBody = pm.response.text();
    pm.expect(responseBody).to.include("Бронирование не найдено");
});
```

**Результат автотестов**
<img width="472" height="177" alt="image" src="https://github.com/user-attachments/assets/e9353c5a-7e3f-45c5-9306-577ffb8ce2a5" />

### 2.3. Отмена бронирования DELETE /api/bookings/{bookingId}

**Тест 1: 200**

<img width="1284" height="395" alt="image" src="https://github.com/user-attachments/assets/7b01f713-3e98-42d9-ae3c-7ee51c0161cd" />
<img width="1278" height="457" alt="image" src="https://github.com/user-attachments/assets/3b4a7c6d-1065-44d0-a6b5-09e991ac55db" />

**Ответ**
<img width="1277" height="377" alt="image" src="https://github.com/user-attachments/assets/eeb6b125-3afb-4775-a540-b2f88cb375e9" />


**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Ответ содержит все обязательные поля", function() {
    const response = pm.response.json();
    
    pm.expect(response).to.have.property("bookingId");
    pm.expect(response).to.have.property("status");
    pm.expect(response).to.have.property("cancelledAt");
    pm.expect(response).to.have.property("cancelledBy");
    pm.expect(response).to.have.property("slotStatus");
    pm.expect(response).to.have.property("notificationSent");
});

```
**Результат автотестов**
<img width="605" height="161" alt="image" src="https://github.com/user-attachments/assets/689f2719-97be-4c43-b2d1-036f07b0d8a2" />

**Тест 2: 404**

<img width="1276" height="383" alt="image" src="https://github.com/user-attachments/assets/572dc9df-22cd-4c7b-9b75-79faa5f575c5" />
<img width="1263" height="394" alt="image" src="https://github.com/user-attachments/assets/7e8c7a54-6966-4b46-8d43-4ecd2d8fafe4" />

**Ответ**
<img width="1285" height="216" alt="image" src="https://github.com/user-attachments/assets/13e1670b-e5ce-422f-95c1-84f6262aed66" />

**Код автотестов**

```
pm.test("Статус ответа 404 Not Found", function() {
    pm.response.to.have.status(404);
});

pm.test("Сообщение об ошибке", function() {
    const responseBody = pm.response.text();
    pm.expect(responseBody).to.include("Бронирование не найдено");
});

```
**Результат автотестов**
<img width="491" height="143" alt="image" src="https://github.com/user-attachments/assets/5ce66da3-6918-4a51-ac15-c038c1f97e2c" />


### 2.4. Получение бронирований преподавателя GET /api/bookings/teachers/{teacherId}/bookings

**Тест 1: 200 (все бронирования)**

<img width="1281" height="324" alt="image" src="https://github.com/user-attachments/assets/9b6133b5-e9b1-4936-9618-6abb3bc3efb8" />

**Ответ**
<img width="1414" height="643" alt="image" src="https://github.com/user-attachments/assets/f029dd7b-4b28-425f-9546-d20dcf50b73c" />

**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Ответ содержит bookings и summary", function() {
    const response = pm.response.json();
    
    pm.expect(response).to.have.property("bookings");
    pm.expect(response).to.have.property("summary");
    pm.expect(response.bookings).to.be.an("array");
});

```
**Результат автотестов**
<img width="568" height="150" alt="image" src="https://github.com/user-attachments/assets/31fe2878-dd6a-4bb3-ac89-1aaebbbacb54" />

**Тест 2: 200 (бронирования с фильтрами)**

<img width="1313" height="394" alt="image" src="https://github.com/user-attachments/assets/6bbb1173-0b98-45ef-8dbe-d0c76131588f" />

**Ответ**
<img width="1275" height="572" alt="image" src="https://github.com/user-attachments/assets/789a6f18-9b60-44a7-96d8-b74c6ee56fce" />

**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Все бронирования имеют статус 2 (Занято)", function() {
    const response = pm.response.json();
    
    if (response.bookings.length > 0) {
        response.bookings.forEach(booking => {
            pm.expect(booking.status).to.be.oneOf([2]);
        });
    }
});

```
**Результат автотестов**
<img width="639" height="201" alt="image" src="https://github.com/user-attachments/assets/01cd9c30-0c24-417c-bf7a-23ac6f3f4f34" />

## 3. Тестирование Notification API
### 3.1. Отправка напоминания POST /api/notifications/reminders

**Тест 1: 201**
<img width="1262" height="402" alt="image" src="https://github.com/user-attachments/assets/cbc68e1c-9f62-4878-af6a-3cd6ca78cd88" />
<img width="1271" height="357" alt="image" src="https://github.com/user-attachments/assets/07ef21bc-f62a-46f2-b967-aa68ca1b7dc5" />

**Ответ**
<img width="1297" height="368" alt="image" src="https://github.com/user-attachments/assets/96387305-327d-4d4e-bf42-5102ea9ecc2b" />


**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Ответ содержит все обязательные поля", function() {
    const response = pm.response.json();
    
    pm.expect(response).to.have.property("notificationId");
    pm.expect(response).to.have.property("sentTo");
    pm.expect(response).to.have.property("sentAt");
    pm.expect(response).to.have.property("nextReminder");
});
```

**Результат автотестов**

<img width="679" height="180" alt="image" src="https://github.com/user-attachments/assets/31fb410b-dbc9-4334-b5dc-64a77239af83" />

**Тест 2: 404**
<img width="1270" height="399" alt="image" src="https://github.com/user-attachments/assets/0616abd2-5c71-4a4d-8c13-9b3ee5997763" />
<img width="1293" height="392" alt="image" src="https://github.com/user-attachments/assets/f60c7d83-a815-42f4-b333-6602dba751f7" />
**Ответ**
<img width="1264" height="242" alt="image" src="https://github.com/user-attachments/assets/e9ab57e1-9aa5-45f5-9a03-2169680f93c9" />

**Код автотестов**

```
pm.test("Статус ответа 404 Not Found", function() {
    pm.response.to.have.status(404);
});

pm.test("Сообщение об ошибке", function() {
    const responseBody = pm.response.text();
    pm.expect(responseBody).to.include("Бронирование не найдено");
});
```

**Результат автотестов**

<img width="517" height="220" alt="image" src="https://github.com/user-attachments/assets/c1b32dc9-2199-40a2-a11b-ca10492a5ef3" />

### 3.2.Получение истории уведомлений GET /api/notifications/history

**Тест 1: 200 - вся история**
<img width="1256" height="315" alt="image" src="https://github.com/user-attachments/assets/b3b8e436-e486-40ef-8ed0-f3db187cd407" />

**Ответ**
<img width="1281" height="556" alt="image" src="https://github.com/user-attachments/assets/9aee7fec-bf41-4d05-a90d-6629f0a79ea7" />

**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Ответ содержит notifications и totalCount", function() {
    const response = pm.response.json();
    
    pm.expect(response).to.have.property("notifications");
    pm.expect(response).to.have.property("totalCount");
    pm.expect(response.notifications).to.be.an("array");
    pm.expect(response.totalCount).to.be.a("number");
});

pm.test("Каждое уведомление содержит обязательные поля", function() {
    const response = pm.response.json();
    
    if (response.notifications.length > 0) {
        const notification = response.notifications[0];
        
        pm.expect(notification).to.have.property("id");
        pm.expect(notification).to.have.property("type");
        pm.expect(notification).to.have.property("recipientId");
        pm.expect(notification).to.have.property("recipientType");
        pm.expect(notification).to.have.property("message");
        pm.expect(notification).to.have.property("sentAt");
        pm.expect(notification).to.have.property("deliveryStatus");
    }
});
```

**Результат автотестов**
<img width="799" height="240" alt="image" src="https://github.com/user-attachments/assets/857afb47-0d46-4972-999f-3c280da8a152" />


**Тест 2: 200 - история с фильтрами**
<img width="1310" height="460" alt="image" src="https://github.com/user-attachments/assets/528ab029-0346-45f9-b784-b92b0885fb34" />

**Ответ**
<img width="1302" height="526" alt="image" src="https://github.com/user-attachments/assets/a422a549-a987-4e5f-b9a8-e72b539ecd25" />


**Код автотестов**

```
pm.test("Статус ответа 200 OK", function() {
    pm.response.to.have.status(200);
});

pm.test("Все уведомления имеют recipientId = student-1", function() {
    const response = pm.response.json();
    
    response.notifications.forEach(notification => {
        pm.expect(notification.recipientId).to.equal("student-1");
    });
});
```

**Результат автотестов**
<img width="615" height="194" alt="image" src="https://github.com/user-attachments/assets/45129a67-232b-4443-a4b9-5cac7df73478" />



