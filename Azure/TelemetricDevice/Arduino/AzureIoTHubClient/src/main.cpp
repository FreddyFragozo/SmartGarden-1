#include <Arduino.h>
#include <WiFi.h>
#include "Esp32MQTTClient.h"
#include "DHT.h"
#include "string.h"

#define DHTPIN 14
#define DHTTYPE DHT11   

//  Wifi settings
const char* ssid     = "";
const char* password = "";
const int waterPin = 32;

//  Iot Hub settings
static const char* connectionString = "";
static bool hasIoTHub = false;
DHT dht(DHTPIN, DHTTYPE);

void setup() {
  Serial.begin(9600);
  Serial.println("Starting connecting WiFi.");
  delay(10);
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  if (!Esp32MQTTClient_Init((const uint8_t*)connectionString)) {
    hasIoTHub = false;
    Serial.println("Initializing IoT hub failed.");
    return;
  }
  
  hasIoTHub = true;
  dht.begin();
  pinMode(waterPin, OUTPUT);
}

void openValve() {
  digitalWrite(waterPin, HIGH);
  delay(10000);
  digitalWrite(waterPin, LOW);
}

void messageCalback(const char * msg, int length) {
  if (strcmp(msg, "OpenValve") == 0) {
    openValve();
  };
}

void loop() {
  Serial.println("Start sending events.");
  
  if (hasIoTHub)
  {
    delay(2000);
    float humidity = dht.readHumidity();
    float temp = dht.readTemperature();
    int water = 0;

    if (isnan(humidity) || isnan(temp)) {
      Serial.println(F("Failed to read from DHT sensor!"));
      return;
    }

    float hic = dht.computeHeatIndex(temp, humidity, false);
    int moisture = analogRead(34);

    if(moisture > 4000)
    {
        Serial.println ("Water Not Detected!");
        Serial.println ("Water Activated");
        water = 1;    
        openValve();    
        Serial.println ("De-activating water");
    }
    else {
        Serial.println ("Water Detected!");
        water = 0;
    }
    
    Serial.print(F("Humidity: "));
    Serial.print(humidity);
    Serial.print(F("%  Temperature: "));
    Serial.print(temp);
    Serial.print(F("°C "));
    Serial.print(F("Heat index: "));
    Serial.print(hic);
    Serial.print(F("°C "));
    Serial.print("Moisture Value : ");
    Serial.println(moisture);

    char buff[128];

    snprintf(buff, 128, "{\"humidity\":%f, \"temperature\":%f, \"heatIndex\":%f, \"deviceId\":\"ESP32\", \"water\":\"%d\"}", humidity, temp, hic, water);
    Serial.println(buff);
    if (Esp32MQTTClient_SendEvent(buff)) {
      Serial.println("Sending data succeed");
    } else {
      Serial.println("Failure...");
    }

    Esp32MQTTClient_SetMessageCallback(messageCalback);
    
    delay(2000);
  }
}
