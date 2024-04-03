[數位發展部全球資訊網](https://moda.gov.tw)
===
數位發展部全球資訊網，體現政府與民眾、政府與政府間傳遞數位發展規劃理念及施政成果的網站，是完整的內容管理系統（Content Management System）；發展之初即規劃整體程式原始碼開放予各界，提供一套可以參考的解決方案，方便應用情境相似的機關團體 Fork 使用；網站以使用者為中心設計、符合網站無障礙規範、提供多國語系架構、支援全站靜態化。

## 開發環境
- 開發工具 : Visual Studio 2022
- 開發框架 : NET 6
- 啟動方式 : Docker

## 專案角色說明
- ConsoleApp    - 排程（靜態化、民意信箱介接、影片清單介接）
- DBModel       - 資料庫模型
- FileServices  - 下載 / 上傳檔案基礎服務
- Management    - 管理後台
- ModaMailBox   - 民意信箱前台
- Services      - 資料庫邏輯
- Utility       - 共用元件
- WebAPI        - API 動態後端
- WebSite       - 動態前台（頁面預覽）

![moda-web-compose](https://github.com/moda-gov-tw/moda-official-website-full/assets/912024/7edce8eb-9ac4-4dce-9cd2-b549d7e2ab20)

## 前置作業

### 設定各專案使用的相關參數（appsettings.json）
* ConsoleApp
```json
{
  "start": "static",
  "ConnectionStrings": {
    "MODA": "Server=${DB_HOST},${DB_PORT};Database=${DB_DATABASE};User ID=${DB_USERNAME};Password=${DB_PASSWORD};"
  },
  "AESKey": "${AESKEY}",
  "NeedEncryption": "0",
  "LogFile": "${LOGFILE}",
  "static": {
    "git": {
      "push": "${GIT_WORKDIR}"
    },
    "WebSiteUrl": "${PUBLIC_WEBSITE_URL}",
    "ResetHours": "4", 
    "IsOfficial": "0",
    "DemoDNS": "${PRIVATE_WEBSITE_URL}"
  },
  "mailbox": {
    "SpeedAPIMore": "create", 
    "ManagementUrl": "${PUBLIC_MANAGEMENT_URL}",
    "ClientID": "${CLIENTID}",
    "ClientSecret": "${CLIENTSECRET}",
    "SpeedAPI": "${SPEEDAPI}",
    "WEBAPI": "${PUBLIC_WEBAPI_URL}/File/Get/",
    "MailBoxUrl": "${PUBLIC_MAILBOX_URL}",
    "FileServiceApi": "${PRIVATE_FILESERVICES_URL}"

  },
  "youtube": {
    "DemoDNS": "${PRIVATE_WEBSITE_URL}"
  },
  "Mail": {
    "sysAdmin": "${MAIL_SYSADMIN}",
    "IsOfficialMail": "true",
    "Default": {
      "Type": "Default",
      "Server": "${MAIL_HOST}",
      "UserName": "${MAIL_USERNAME}",
      "Password": "${MAIL_PASSWORD}",
      "From": "${MAIL_FROM_ADDRESS}",
      "DisplayName": "${MAIL_FROM_NAME}",
      "Port": "${MAIL_PORT}",
      "SSL": "${MAIL_SSL}",
      "IsAccountPWD": "true"
    },
    "MailBox": {
      "Type": "MailBox",
      "Server": "${MAIL_HOST}",
      "UserName": "${MAIL_USERNAME}",
      "Password": "${MAIL_PASSWORD}",
      "From": "${MAIL_FROM_ADDRESS}",
      "DisplayName": "${MAIL_FROM_NAME}",
      "Port": "${MAIL_PORT}",
      "SSL": "${MAIL_SSL}",
      "IsAccountPWD": "true"
    }
  },
}
```
* Management
```json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MODA": "Server=${DB_HOST},${DB_PORT};Database=${DB_DATABASE};User ID=${DB_USERNAME};Password=${DB_PASSWORD};"
  },
  "AESKey": "${AESKEY}",
  "NeedEncryption": "0",
  "MainWebSiteID": "MODA",
  "Mail": {
    "sysAdmin": "",
    "IsOfficialMail": "true",
    "Default": {
      "Type": "Default",
      "Server": "${MAIL_HOST}",
      "UserName": "${MAIL_USERNAME}",
      "Password": "${MAIL_PASSWORD}",
      "From": "${MAIL_FROM_ADDRESS}",
      "DisplayName": "${MAIL_FROM_NAME}",
      "Port": "${MAIL_PORT}",
      "SSL": "${MAIL_SSL}",
      "IsAccountPWD": "true"
    },
    "MailBox": {
      "Type": "MailBox",
      "Server": "${MAILBOX_HOST}",
      "UserName": "${MAILBOX_USERNAME}",
      "Password": "${MAILBOX_PASSWORD}",
      "From": "${MAILBOX_FROM_ADDRESS}",
      "DisplayName": "${MAILBOX_FROM_NAME}",
      "Port": "${MAILBOX_PORT}",
      "SSL": "${MAILBOX_SSL}",
      "IsAccountPWD": "true"
    }
  },
  "StaticPath": "${STATIC_PATH}",
  "FileServiceApi": "${PRIVATE_FILESERVICES_URL}",
  "WEBAPI": "${PUBLIC_WEBAPI_URL}/File/Get/",
  "DemoDNS": "${PRIVATE_WEBSITE_URL}",
  "LocalUrl": "${PUBLIC_MANAGEMENT_URL}",
  "WebSiteUrl": "${PUBLIC_WEBSITE_URL}",
  "MailBoxUrl": "${PUBLIC_MAILBOX_URL}",
  "ClientID": "${CLIENTID}",
  "ClientSecret": "${CLIENTSECRET}",
  "SpeedAPI": "${SPEEDAPI}",
  "OpenAzureAD": "${OPENAZUREAD}",
  "Azure_tanentid": "${AZURE_TANENTID}",
  "Azure_clientid": "${AZURE_CLIENTID}",
  "Azure_secret": "${AZURE_SECRET}",
  "Azure_callback_url": "${AZURE_CALLBACK_URL}",
  "DataProtectionKey": {
    "applicationName": "${APPLICATIONNAME}",
    "confirmKey": "${CONFIRMKEY}",
    "surveyKey": "${SURVEYKEY}"
  },
  "Environment": "linux"
}
```
* ModaMailBox
``` json
{
  "ConnectionStrings": {
    "MODA": "Server=${DB_HOST},${DB_PORT};Database=${DB_DATABASE};User ID=${DB_USERNAME};Password=${DB_PASSWORD};"
  },
  "AESKey": "${AESKEY}",
  "NeedEncryption": "0",
  "Mail": {
    "sysAdmin": "${SYSADMIN}",
    "IsOfficialMail": "true",
    "Default": {
      "Type": "Default",
      "Server": "${MAIL_HOST}",
      "UserName": "${MAIL_USERNAME}",
      "Password": "${MAIL_PASSWORD}",
      "From": "${MAIL_FROM_ADDRESS}",
      "DisplayName": "${MAIL_FROM_NAME}",
      "Port": "${MAIL_PORT}",
      "SSL": "${MAIL_SSL}",
      "IsAccountPWD": "true"
    },
    "MailBox": {
      "Type": "MailBox",
      "Server": "${MAILBOX_HOST}",
      "UserName": "${MAILBOX_USERNAME}",
      "Password": "${MAILBOX_PASSWORD}",
      "From": "${MAILBOX_FROM_ADDRESS}",
      "DisplayName": "${MAILBOX_FROM_NAME}",
      "Port": "${MAILBOX_PORT}",
      "SSL": "${MAILBOX_SSL}",
      "IsAccountPWD": "true"
    }
  },
  "EffectiveHours": "${EFFECTIVEHOURS}",
  "EffectiveDays": "${EFFECTIVEDAYS}",
  "AllowedHosts": "*",
  "FileServiceApi": "${PRIVATE_FILESERVICES_URL}",
  "WEBAPI": "${PUBLIC_WEBAPI_URL}/File/Get/",
  "WEBSiteUrl": "${PUBLIC_WEBSITE_URL}",
  "localUrl": "${PUBLIC_MAILBOX_URL}",
  "IsScan": "${ISSCAN}",
  "Official": "0",
  "tempFile": "${TEMPFILE}",
  "virusscan": "${VIRUSSCAN}",
  "ClientID": "${CLIENTID}",
  "ClientSecret": "${CLIENTSECRET}",
  "speedAPI": "${SPEEDAPI}",
  "CloudFlareTurnstileSitekey": "${CLOUDFLARETURNSTILESITEKEY}",
  "CloudFlareTurnstileSecretkey": "${CLOUDFLARETURNSTILESECRETKEY}",
  "Content-Security-Policy": "${CONTENT_SECURITY_POLICY}",
  "antivirus": "clamav"
}
```
* WebAPI
``` json
{
  "ConnectionStrings": {
    "MODA": "Server=${DB_HOST},${DB_PORT};Database=${DB_DATABASE};User ID=${DB_USERNAME};Password=${DB_PASSWORD};"
  },
  "AESKey": "${AESKEY}",
  "NeedEncryption": "0",
  "AllowedHosts": "*",
  "AllowOrigins": [
     "${ALLOWORIGINS}"
  ],
  "FileServiceApi": "${PRIVATE_FILESERVICES_URL}",
  "WebSiteHost": "${PUBLIC_WEBSITE_URL}",
  "WebAPIUrl": "${PUBLIC_WEBAPI_URL}/File/Get/",
  "AllowSwagger": "false",
}
```
* WebSite
``` json
{
  "ConnectionStrings": {
    "MODA": "Server=${DB_HOST},${DB_PORT};Database=${DB_DATABASE};User ID=${DB_USERNAME};Password=${DB_PASSWORD};"
  },
  "MainWebSite": "MODA",
  "MainLang": "zh-tw",
  "AESKey": "${AESKEY}",
  "NeedEncryption": "0",
  "AllowedHosts": "*",
  "WebSiteUrl": "${PUBLIC_WEBSITE_URL}",
  "WebAPIUrl": "${PUBLIC_WEBAPI_URL}",
  "IsStatic": "1"
}
```

* FileServices
``` json
{
  "AllowedHosts": "*",
  "Upload": "${UPLOAD}"
}
```

### appsettings 組態設定檔相關參數說明

- 基本參數說明

| 必填 | 參數名稱 | 範例 | 說明 |
|-|-|-|-|
| * | start | static | 啟動類型：<br />"static": 靜態化<br />"mailbox": 民意信箱介接<br />"youtube": 影片清單介接 |
| * | NeedEncryption | 0 | 內部參數是否需要 AESKey 加密：<br />"0": 否<br />"1": 是 |
| * | ResetHours | 4 | 每日於幾時進行全站靜態化排程（24 小時制） |
| * | IsOfficial | 0 | 產生 sitemap 時是否將網址後綴 .html 去掉：<br />"0": 否<br />"1": 是 |
| * | SpeedAPIMore | create | 民意信箱排程介接模式參數：<br />"create": 新增<br />"search": 查詢 |
| * | IsOfficialMail | true | 是否為正式寄信：<br />"true": 是<br />"false": 否 |
| * | MainWebSiteID | MODA | 主站台代碼 |
| * | DataProtectionKey.applicationName | | 民意信箱功能啟用的應用程式名稱 |
| * | DataProtectionKey.confirmKey | | 民意信箱案件確認信時效的加密金鑰 |
| * | DataProtectionKey.surveyKey | | 民意信箱滿意度調查時效的加密金鑰 |
| * | Environment | linux | 作業環境：<br />"linux"<br />"windows" |
| $ | antivirus | clamav | 啟用的防毒軟體名稱（`IsScan` 為 "1" 時必填） |
| * | IsScan | 1 | 是否啟用掃毒軟體掃描毒上傳檔案：<br />"1": 啟用<br />"0": 關閉
| * | MainLang | zh-tw | 主要語系 |
| * | IsStatic | 1 | 不對外公開的動態前台（頁面預覽）的網站連結網址路徑置換功能：<br />"0": 測試模式（動態前台維持原本可互動連結網址）<br />"1": 正式模式（連結網址置換為 SSG 需要的靜態路徑） |
| * | DemoDNS | | 例如：不對外公開的動態前台（頁面預覽）網址 |
| * | WebSiteUrl | | 例如：公開的靜態路徑前台網址 |
| | ClientID | | 民意信箱介接外部系統提供 ID |
| | ClientSecret | | 民意信箱介接外部系統提供 Secret |
|* | IsAccountPWD| true | 透過寄信服務寄信時是否需要密碼（"true" 則 `UserName` 及 `Password` 必填） :<br />"true": 是<br />"false": 否 |
|$ | UserName | test | 寄信服務寄信使用的帳號 |
|$ | Password | test | 寄信服務寄信使用的密碼 |



`*` = required
`$` = optional


- 外部化參數及環境變數（docker-compose 使用的 .env 檔）說明


| 必填 | 變數名稱 | 範例 | 說明 |
|:-:|-|:-:|:-:|
| | AESKEY | | 系統加密金鑰，如需使用請自行調整程式 |
| * | ALLOWORIGINS | http://localhost | 跨源資源存取政策（CORS）設定：<br />`*`（允許所有來源）<br />`https://*`（允許以 https:// 開頭的任何來源）<br />`http://*`（允許以 http:// 開頭的任何來源） |
| * | APPLICATIONNAME | MailBox | 民意信箱功能啟用的應用程式名稱 |
| * | ASPNETCORE_ENVIRONMENT | Development | 民意信箱前台若需要遵守 CSP 建議改為 Production（預設是 Development 環境）：<br />`Development`（低安全）<br />`Staging`（中安全）<br />`Production`（高安全）<br />此變數設定 `Staging` 及`Production` 會增加如下的安全性設定：<br />1. Content-Security-Policy: ...<br />2. X-Frame-Options: SAMEORIGIN<br />3. X-Content-Type-Options : nosniff<br />4. Referrer-Policy: no-referrer |
| | AZURE_CALLBACK_URL | | AzureAD 登入介接的回呼網址 |
| | AZURE_CLIENTID | | AzureAD 登入介接的 ClientID 資訊 |
| | AZURE_SECRET | | AzureAD 登入介接的 Secret 資訊 |
| | AZURE_TANENTID | | AzureAD 登入介接的 TanentID 資訊 |
| | CLIENTID | | 民意信箱介接外部系統提供 ID |
| | CLIENTSECRET | | 民意信箱介接外部系統提供 Secret |
| | CLOUDFLARETURNSTILESECRETKEY | 1x00000000000000000000AA | Cloudflare Turnstile 驗證 Secret（測試 Key） |
| | CLOUDFLARETURNSTILESITEKEY | 1x0000000000000000000000000000000AA | Cloudflare Turnstile 驗證（測試 Key） |
| | CONFIRMKEY | confirm | 民意信箱案件確認信時效的加密金鑰 |
| * | CONTENT_SECURITY_POLICY | default-src * 'self' 'unsafe-inline' 'unsafe-eval' data: gap: content:; img-src 'self' blob: data: http://localhost ;frame-src *;script-src * 'unsafe-inline' 'unsafe-eval';style-src * 'unsafe-inline' 'unsafe-eval';frame-ancestors 'self';font-src 'self' data: https://cdn.jsdelivr.net https://fonts.gstatic.com ;worker-src blob:; child-src blob: gap:; | 站台 CSP 相關參數設定 |
| * | DB_DATABASE | DEMO_MODA | 資料庫名稱 |
| * | DB_HOST | 127.0.0.1 | 資料庫管理系統主機資訊 |
| * | DB_USERNAME | sa | 資料庫管理系統登入帳號 |
| * | DB_PASSWORD | P@ssword1234 | 資料庫管理系統登入密碼 |
| * | DB_PORT | 1433 | 資料庫管理系統主機連接埠資訊 |
| * | EFFECTIVEDAYS | 3 | 民意信箱意見回饋有效時間（天） |
| * | EFFECTIVEHOURS | 2 | 民意信箱驗證有效時間（小時） |
| | ESETPATH | ecls.exe | ESET 執行檔位置 |
| | GA4LANGUAGE | zh-tw | 站台語系（若 GA4CODE 有串必填）：<br />`zh-tw`: 中文<br />`en`: 英文 |
| | GA4CODE | | GA4 代碼（請至 Google GA4 管理介面查詢） |
| | GIT_REPO | | 遠端儲存庫 URI |
| * | GIT_WORKDIR | /app/files/wwwroot | 靜態化排程的網頁儲存及待推送的儲存空間目錄 |
| | GOOGLE_AUTH_PROVIDER_X509_CERT_URL | | GA4 資訊介接（取自 Google API 金鑰裡面的 auth_provider_x509_cert_url） |
| | GOOGLE_AUTH_URI | | GA4 資訊介接（取自 Google API 金鑰裡面的 auth_uri） |
| | GOOGLE_CLIENT_EMAIL | | GA4 資訊介接（取自 Google API 金鑰裡面的 private_key） |
| | GOOGLE_CLIENT_ID | | GA4 資訊介接（取自 Google API 金鑰裡面的 client_id） |
| | GOOGLE_CLIENT_X509_CERT_URL | | GA4 資訊介接（取自 Google API 金鑰裡面的 auth_provider_x509_cert_url） |
| | GOOGLE_PRIVATE_KEY | | GA4 資訊介接（取自 Google API 金鑰裡面的 private_key） |
| | GOOGLE_PRIVATE_KEY_ID | | GA4 資訊介接（取自 Google API 金鑰裡面的 private_key_id） |
| | GOOGLE_PROJECT_ID | | GA4 資訊介接（取自 Google API 金鑰裡面的 project_id） |
| | GOOGLE_TOKEN_URI | | GA4 資訊介接（取自 Google API 金鑰裡面的 token_uri） |
| * | ISSCAN | 0 | 是否啟用防毒軟體掃描上傳檔案：<br />"1": 啟動<br />"0": 關閉 |
| * | LOGFILE | log | log 存放位置 |
| | MAILBOX_FROM_ADDRESS | hello@example.com | 民意信箱寄信使用的寄信人信箱 |
| | MAILBOX_FROM_NAME | 測試寄信者 | 民意信箱寄信使用的寄信人名稱 |
| | MAILBOX_HOST | maildev | 民意信箱寄信使用的寄信服務主機資訊 |
| | MAILBOX_PASSWORD | test | 民意信箱寄信使用的寄信服務密碼 |
| | MAILBOX_PORT | 1025 | 民意信箱寄信使用的寄信服務連接埠資訊 |
| | MAILBOX_USERNAME | test | 民意信箱寄信使用的寄信服務帳號 |
| | MAILBOX_SSL | true | 民意信箱是否使用 TLS 的寄信服務：<br />"true": 是<br />"false": 否 |
| * | MAIL_FROM_ADDRESS | hello@example.com | 寄信使用的寄信人信箱 |
| * | MAIL_FROM_NAME | 測試寄信者 | 寄信使用的寄信人名稱 |
| * | MAIL_HOST | maildev | 寄信使用的寄信服務主機資訊 |
| * | MAIL_PASSWORD | test | 寄信使用的寄信服務密碼 |
| * | MAIL_PORT | 1025 | 寄信使用的寄信服務連接埠資訊 |
| * | MAIL_SSL | true | 是否使用 TLS 的寄信服務：<br />"true": 是<br />"false": 否 |
| * | MAIL_USERNAME | test | 寄信使用的寄信服務帳號 |
| | MAIL_SYSADMIN | hello@example.com | 系統預設管理員信箱 |
| * | OPENAZUREAD | 0 | 登入機制：<br />"0": 帳號密碼登入<br />"1": AzureAD 登入 |
| * | PRIVATE_FILESERVICES_URL | http://fileservies | 不對外公開的檔案服務網址 |
| * | PRIVATE_WEBSITE_URL | http://website | 不對外公開的動態前台（頁面預覽）網址 |
| * | PUBLIC_MAILBOX_URL | http://localhost:8003 | 對外公開的民意信箱前台網址 |
| * | PUBLIC_MANAGEMENT_URL | http://localhost:8002 | 對外公開的管理後台網址 |
| * | PUBLIC_WEBAPI_URL | http://localhost:8001 | 對外公開的 API 動態後端網址 |
| * | PUBLIC_WEBSITE_URL | http://localhost:8000 | 對外公開的靜態路徑前台網址 |
| | SPEEDAPI | | 民意信箱介接外部系統網址 |
| * | STATIC_PATH | files/wwwroot | 靜態圖檔位置 |
| | SURVEYKEY | survey | 民意信箱滿意度調查時效的加密金鑰 |
| | TCPADDR | | 防毒服務主機資訊 |
| * | TEMPFILE | /tmp | 民意信箱上傳檔掃毒的暫存目錄 |
| * | UPLOAD | files | 下載 / 上傳檔案基礎服務的儲存空間目錄 |
| | VIRUSSCAN | /app/clamdtest.sh | 執行防毒掃描腳本的位置 |
| | GITHUB_KEY | | GitHub 使用的 Deploy Key 私鑰 |


`*` = required


## 專案建置
### 非 Visual Studio  工具
可參考 https://learn.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference 進行更新



## 建置注意事項

### Azure AD SSO
Management 專案的登入機制可與 Azure AD 串單一登入，若想使用帳密登入請調整 appsettings.json 的 `OpenAzureAD` 參數為 `0` 
> 實作 Azure AD 登入機制參考
> https://learn.microsoft.com/zh-tw/azure/active-directory/fundamentals/active-directory-whatis

需設定 appsettings.json 的參數 `Azure_tanentid` + `Azure_clientid` + `Azure_secret` + `Azure_callback_url`


### 必要工作
- 站台建立完成後，請將 `user-img` 內的 `copyright` 目錄複製至儲存空間的 `wwwroot` 下。
- 站台建立完成後，匯入基礎資料請使用容器 `mssql-tools` 執行如下的指令：
 ```shell
  # docker-compose exec mssql-tools bash
  # sqlcmd -S mssql -U sa -P P@ssword1234 -i /srv/db/DEMO_MODA.sql -x
  ```
- 確認管理後台在 appsettings.json 中的參數 `StaticPath` 所設定的路徑下要有 `copyright` 目錄，否則啟動 Management.dll 會報錯。
- 沒有建置防毒服務的情況下，民意信箱在 appsettings.json 中關於檔案掃毒的參數 `IsScan` 請設為 `0`，否則會造成民意信箱上傳附件出錯。
- 若未給予參數 `GIT_REPO` `GITHUB_KEY` `GITHUB_PUBKEY`，靜態化排程在執行 `push.sh` 時會出現錯誤訊息，雖不影響靜態化結果，但可以透過註解 ConsoleApp-Static/docker-entrypount.sh 內的 /app/push.sh 指令後重新建置 Image 解決。

## Docker Compose

### 安裝

依照不同作業系統可以從 [Docker](https://docs.docker.com/compose/install/) 找到相對應的安裝說明，以下以 Linux 環境為例 : 

1. 先從官方 [github repo](https://github.com/docker/compose/releases/latest) 下載最新版本的程式放到 `/usr/local/bin/` 路徑下並命名為 `docker-compose`：
```shell
# curl -SL https://github.com/docker/compose/releases/download/2.26.0/docker-compose-linux-x86_64 -o /usr/local/bin/docker-compose
```
2. 授予 `/usr/local/bin/docker-compose` 執行權限：
```shell
# chmod +x /usr/local/bin/docker-compose
```
3. 可藉由執行指令`docker-compose version` 測試是否有安裝成功：
```shell
# docker-compose version
Docker Compose version v2.26.0
```
### 環境建置
在方案目錄裡的 docker-compose.yml 可以指定 Image 或 Dockerfile 同時建置複數個 Container。

使用 `docker-compose build` 指令來建置所有定義好的 Image：
```shell
# docker-compose build 
[+] Building 597.3s (74/74) FINISHED 
 => [website internal] load .dockerignore 
 => => transferring context: 48B             
 => [website internal] load build definition from Dockerfile   
 => => transferring dockerfile: 514B
 ......
```

使用 `docker-compose up -d` 指令來啟動所有服務，並放到背景執行：
```shell
# docker-compose up -d

 ✔ Network moda-website_default                  Created
 ✔ Container moda-website-website-1              Started
 ✔ Container moda-website-consoleapp-static-1    Started 
 ✔ Container moda-website-fileservices-1         Started 
 ✔ Container moda-website-maildev-1              Started 
 ✔ Container moda-website-adminer-1              Started 
 ✔ Container moda-website-mssql-tools-1          Started
 ✔ Container moda-website-mailbox-1              Started
 ✔ Container moda-website-management-1           Started
 ✔ Container moda-website-mssql-1                Started 
 ✔ Container moda-website-webapi-1               Started
```
可以使用 `docker-compose ps ` 指令確認服務是否有正常啟動，`STATUS` 欄位顯示 `UP` 代表我們的服務有正常在執行，其中 `consoleapp-static` 未出現在列表內是因為靜態化執行完成後就會自動結束服務：
```shell
NAME                          IMAGE                                        COMMAND                                                         SERVICE        CREATED              STATUS                        PORTS
moda-website-adminer-1        adminer:4.8.1                                "entrypoint.sh php -S [::]:8080 -t /var/www/html"               adminer        About a minute ago   Up About a minute             0.0.0.0:8080->8080/tcp
moda-website-fileservices-1   moda-website-fileservices                    "/docker-entrypoint.sh"                                         fileservices   About a minute ago   Up About a minute             80/tcp
moda-website-mailbox-1        moda-website-mailbox                         "/docker-entrypoint.sh"                                         mailbox        About a minute ago   Up About a minute             0.0.0.0:8003->80/tcp
moda-website-maildev-1        maildev/maildev:latest                       "bin/maildev"                                                   maildev        About a minute ago   Up About a minute (healthy)   1025/tcp, 0.0.0.0:1080->1080/tcp
moda-website-management-1     moda-website-management                      "/docker-entrypoint.sh"                                         management     About a minute ago   Up About a minute             0.0.0.0:8002->80/tcp
moda-website-mssql-1          mcr.microsoft.com/mssql/server:2019-latest   "/opt/mssql/bin/permissions_check.sh /opt/mssql/bin/sqlservr"   mssql          About a minute ago   Up About a minute             1433/tcp
moda-website-mssql-tools-1    mcr.microsoft.com/mssql-tools                "tail -f /dev/null"                                             mssql-tools    About a minute ago   Up About a minute
moda-website-webapi-1         moda-website-webapi                          "/docker-entrypoint.sh"                                         webapi         About a minute ago   Up About a minute             0.0.0.0:8001->80/tcp
moda-website-website-1        moda-website-website                         "/docker-entrypoint.sh"                                         website        About a minute ago   Up About a minute             0.0.0.0:8000->80/tcp
```
### 終止服務
使用 `docker-compose down` 指令終止服務：
```shell
 ✔ Container moda-website-mssql-tools-1          Removed
 ✔ Container moda-website-adminer-1              Removed
 ✔ Container moda-website-website-1              Removed 
 ✔ Container moda-website-consoleapp-static-1    Removed 
 ✔ Container moda-website-management-1           Removed
 ✔ Container moda-website-maildev-1              Removed 
 ✔ Container moda-website-webapi-1               Removed
 ✔ Container moda-website-mailbox-1              Removed 
 ✔ Container moda-website-fileservices-1         Removed
 ✔ Container moda-website-mssql-1                Removed 
 ✔ Network moda-website_default                  Removed
```

---

## Docker Compose 服務說明

### WebSite
將不對外公開的動態前台網站的連結網址置換後，使之公開為靜態路徑的動態前台網站。
- 端點：http://localhost:8000

### Management
全球資訊網管理後台，預設登入的帳號：`demo`、密碼：`demo`。
- 端點：http://localhost:8002

### ModaMailBox
民意信箱前台。
- 端點：http://localhost:8003

### MailDev
開發時期用於測試電子郵件功能的虛擬 SMTP 伺服器及網頁郵件服務，在專案的 appsettings.json 設定好對應的寄信設定後，當專案啟動進行寄信時，即可在 MailDev 的網頁介面收信。
- 端點：http://localhost:1080

### Adminer
單隻 PHP 程式的網頁版資料庫管理工具，資料庫管理系統啟動後可以在 Adminer 使用 appsettings.json 中的連線字串登入資料庫，由網頁介面進行管理。
- 端點：http://localhost:8080

### SQL Server
開發時期使用的資料庫管理系統，使用容器版可以方便的部署和執行 Microsoft SQL Server ，並使其在不同環境中保持一致。預設登入帳號：`sa`、密碼：`P@ssword1234`。

### mssql-tool
mssql-tool 是一個針對 Microsoft SQL Server 的命令列工具，可以在容器內使用 sqlcmd 指令與資料庫進行溝通；進入容器後可以使用指令，進入指令為： `docker-compose exec -it mssql-tools bash` 。

### ConsoleApp-static
由 ConsoleApp 所拆分出來的網頁靜態化的排程服務。

### ConsoleApp-youtube
由 ConsoleApp 所拆分出來的 YouTube 影片清單更新的排程服務。

### ConsoleApp-create
由 ConsoleApp 所分出來的民意信箱介接外部系統建案的排程服務。

### ConsoleApp-search 
由 ConsoleApp 所分出來的民意信箱介接外部系統狀態更新的排程服務。

---
## License

本專案採用 MIT 授權宣告，以下為第三方軟體元件授權清單：

| package                                                      | license                                                                                    | comment  |
|--------------------------------------------------------------|--------------------------------------------------------------------------------------------|----------|
|     jquery-3.6.0.min.js                                      |     MIT                                                                                    |          |
|     jquery-ui.js                                             |     MIT                                                                                    |          |
|     jquery-validation-unobtrusive.js                         |     MIT                                                                                    |          |
|     jquery.magnific-popup.min.js                             |     MIT                                                                                    |          |
|     jquery.unobtrusive-ajax.min.js                           |     Apache                                                                                 |          |
|     jquery.countTo.min.js                                    |     MIT                                                                                    |          |
|     Bootstrap                                                |     MIT                                                                                    |          |
|     bootstrap-submenu.js                                     |     MIT                                                                                    |          |
|     aos.js                                                   |     MIT                                                                                    |          |
|     lazyload.min.js                                          |     MIT                                                                                    |          |
|     swiper-bundle.min.js                                     |     MIT                                                                                    |          |
|     waves.js                                                 |     MIT                                                                                    |          |
|     metismenu.js                                             |     MIT                                                                                    |          |
|     piexif.js                                                |     MIT                                                                                    |          |
|     sortable.js                                              |     MIT                                                                                    |          |
|     fileinput.js                                             |     BSD-3- Clause                                                                          |          |
|     sweetalert2.min.js                                       |     MIT                                                                                    |          |
|     ckeditor.js                                              |     MPL                                                                                    |          |
|     chart.js                                                 |     MIT                                                                                    |          |
|     prism.js                                                 |     MIT                                                                                    |          |
|     Font Awesome Free                                        |     CC BY 4.0                                                                              |          |
|     Noto Sans TC                                             |     SIL OFL 1.1                                                                            |          |
|     Google.Analytics.Data.V1Beta                             |     Apache                                                                                 |          |
|     Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation        |     MIT                                                                                    |          |
|     Microsoft.Extensions.Caching.Memory                      |     MIT                                                                                    |          |
|     Microsoft.VisualStudio.Azure.Containers.Tools.Targets    |     MICROSOFT SOFTWARE LICENSE TERMS    MICROSOFT VISUAL STUDIO CONTAINER TOOLS TARGETS    | 授權軟體 |
|     Microsoft.VisualStudio.Web.CodeGeneration.Design         |     MIT                                                                                    |          |
|     Microsoft.Extensions.Configuration.Json                  |     MIT                                                                                    |          |
|     NetTopologySuite.IO.GeoJSON                              |     BSD 3-Clause                                                                           |          |
|     NuGet.Protocol                                           |     Apache                                                                                 |          |
|     Swashbuckle.AspNetCore                                   |     MIT                                                                                    |          |
|     System.DirectoryServices                                 |     MIT                                                                                    |          |
|     System.Net.Http                                          |     MICROSOFT SOFTWARE LICENSE TERMS    MICROSOFT .NET LIBRARY                             |          |
