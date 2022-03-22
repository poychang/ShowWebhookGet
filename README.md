# Show Webhook Get

在網站中顯示 Webhook 所拿到的資料內容。

# 使用情境

透過 Webhook 可以讓第三方平台串接回自己的站台，並且可以讓第三方平台接收到訊息後，進行處理，再回傳給第三方平台。

第三方平台在呼叫我們的 Webhook 端點時，會傳送指定的資料內容，讓我們可以做後續處理，這個網站就是方便我們開發時查看 Webhook 的資料內容。

1. 開啟網站 [showwebhookget.azurewebsites.net](https://showwebhookget.azurewebsites.net)
2. 讓第三方平台呼叫 `https://showwebhookget.azurewebsites.net/api/webhook` Webhook 端點，或用 Postman 來呼叫下列範例
   - POST 
   - Body { "message": "Hello World" }https://showwebhookget.azurewebsites.net/api/webhook
3. 網站會顯示 `{ "message": "Hello World" }`

## 技術

- 後端使用 ASP.NET Core Minimal Web API 建置
- 前端使用 [Server-sent Event](https://developer.mozilla.org/zh-TW/docs/Web/API/Server-sent_events/Using_server-sent_events) 來即時呈現 Webhook 拿到的資料
