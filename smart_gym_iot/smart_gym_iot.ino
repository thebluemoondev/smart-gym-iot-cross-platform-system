#include <SPI.h>
#include <MFRC522.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <WiFiClientSecure.h> 

#define SS_PIN    5
#define RST_PIN   27 
#define BUZZER    4
#define LED_G     15
#define LED_R     2

MFRC522 rfrc(SS_PIN, RST_PIN);
LiquidCrystal_I2C lcd(0x27, 16, 2); 

const char* ssid = "bluemoon";
const char* password = "thanhchinh";

const char* serverUrl = "https://api.thanhchinh.io.vn/api/Rfid/check-in";

void setup() {
  Serial.begin(115200);
  SPI.begin();
  rfrc.PCD_Init();
  rfrc.PCD_SetAntennaGain(rfrc.RxGain_max); 
  
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Smart Gym System");
  lcd.setCursor(0, 1);
  lcd.print("Connect WiFi...");

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  lcd.clear();
  lcd.print("Ready to Scan!");
  Serial.println("\nWiFi Connected! IP: " + WiFi.localIP().toString());
  
  pinMode(BUZZER, OUTPUT);
  pinMode(LED_G, OUTPUT);
  pinMode(LED_R, OUTPUT);
  
  digitalWrite(BUZZER, HIGH); delay(100); digitalWrite(BUZZER, LOW);
}

void loop() {
  if (!rfrc.PICC_IsNewCardPresent() || !rfrc.PICC_ReadCardSerial()) return;

  String uid = "";
  for (byte i = 0; i < rfrc.uid.size; i++) {
    uid += String(rfrc.uid.uidByte[i] < 0x10 ? "0" : "");
    uid += String(rfrc.uid.uidByte[i], HEX);
  }
  uid.toUpperCase();
  
  Serial.println("\n-----------------------");
  Serial.println("Quet the: " + uid);
  
  lcd.clear();
  lcd.print("Checking...");
  
  sendRequestToServer(uid);
  
  rfrc.PICC_HaltA();
  rfrc.PCD_StopCrypto1();
}

void sendRequestToServer(String cardUid) {
  if (WiFi.status() == WL_CONNECTED) {
    WiFiClientSecure client;
    client.setInsecure(); 

    HTTPClient http;
    
    http.begin(client, serverUrl); 
    http.addHeader("Content-Type", "application/json");
    http.setTimeout(5000); 

    StaticJsonDocument<128> doc;
    doc["cardUid"] = cardUid; 
    String jsonRequest;
    serializeJson(doc, jsonRequest);

    int httpCode = http.POST(jsonRequest);
    Serial.print("HTTP Code: "); Serial.println(httpCode);

    if (httpCode == 200) {
      String response = http.getString();
      Serial.println("Response: " + response);
      
      StaticJsonDocument<512> resDoc;
      DeserializationError error = deserializeJson(resDoc, response);

      if (!error) {
        String status = resDoc["status"];
        String name = resDoc["name"];
        String msg = resDoc["message"];

        lcd.clear();
        if (status == "Success") {
          lcd.setCursor(0, 0); lcd.print("Chao: " + name);
          lcd.setCursor(0, 1); lcd.print("Moi vao tap!");
          handleSuccess();
        } else {
          lcd.setCursor(0, 0); lcd.print("TU CHOI!");
          lcd.setCursor(0, 1); lcd.print(msg);
          handleFail();
        }
      }
    } else {
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("Loi: "); lcd.print(httpCode);
      if (httpCode == -1) {
          lcd.setCursor(0, 1); lcd.print("No Internet?");
      }
      handleFail();
    }
    http.end();
  } else {
    lcd.clear();
    lcd.print("WiFi Lost!");
  }
}

void handleSuccess() {
  digitalWrite(LED_G, HIGH);
  digitalWrite(BUZZER, HIGH); 
  delay(200);
  digitalWrite(BUZZER, LOW);
  delay(1500);
  digitalWrite(LED_G, LOW);
  lcd.clear(); lcd.print("Ready to Scan!");
}

void handleFail() {
  digitalWrite(LED_R, HIGH);
  for(int i = 0; i < 3; i++) {
    digitalWrite(BUZZER, HIGH); delay(80);
    digitalWrite(BUZZER, LOW); delay(80);
  }
  delay(1000);
  digitalWrite(LED_R, LOW);
  lcd.clear(); lcd.print("Ready to Scan!");
}