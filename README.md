# eStation Developer Edition
Welcome to eStation Developer Edition! 

eStation is designed for developers to quick integerate ETAG ESL&DSL with their projects. eStation use MQTT protocol and esay to configure/integerate.

Release Date: 2026-04-13

Firmware: 1.1.44

# 1. Work with Mosquitto

If you are working with Mosquitton, you need edit the Mosquitto configure file mosquitto.conf:

allow_anonymous false  
password_file Your_Password_File_Path_Here  # Pasword file path
listener XXXX                               # The MQTT port
max_topic_alias 255                         # The default value is 10, need change to 255

# 2. Understand the Image size
...

# 3. Understand the E-ink screen
...
