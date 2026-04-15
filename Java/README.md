# eStation Java Demo

This directory is the Java port of the Python MQTT demo under `../Python`.

## What is included

- Java equivalents for the Python enums, config constants, and entity classes
- A small MessagePack encoder/decoder used by the entity classes
- A BMP-to-BGRA reader based on the Python `fileHelper.py` behavior
- A runnable `DemoApp` that mirrors the Python menu-driven MQTT workflow

## Project layout

- `src/main/java/com/estation/demo/DemoApp.java`: main entry point
- `src/main/java/com/estation/demo/entities`: payload models
- `src/main/java/com/estation/demo/enums`: enum mappings
- `src/main/java/com/estation/demo/util`: image, hash, and MessagePack helpers

## Run

1. Install Maven if it is not already available.
2. From this `Java` directory, run `mvn package`.
3. To launch the demo, run `java -jar target/estation-demo-1.0.0.jar`.

Alternatively, run directly with Maven using `mvn exec:java`.

The default sample image paths point at the local `src/main/java/com/estation/demo/images` directory.
