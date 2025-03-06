# 關於SmartMan2FA.exe的使用方式:
1. 產生QRCode: `-g -a FrankHuang -k mySecretKey`
2. 驗證PIN: `-v -p 123456 -k mySecretKey` 
3. 產生金鑰: `-c` (產生的金鑰內容將會輸出在secret_key.txt檔案，內只有一行文字，建議讀取完畢後立即刪除此檔)

SmartMan2FA.exe會把產生的Setup code寫到相同目錄底下的setup_code.txt，內只有一行且單獨一組Setup Code。相同目錄底下也會產生QRCode圖片，檔名固定且單一為qrcode.jpg。

(-a) Account為識別用戶的帳號資訊、(-k) Secret key為金鑰、Setup Code為註冊雙重驗證其中一種方式所需要的代碼 (若使用掃描QRCode就可以不用此Setup Code、(-p) PIN為手機驗證器上顯示的六碼數字
