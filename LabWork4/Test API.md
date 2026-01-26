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



