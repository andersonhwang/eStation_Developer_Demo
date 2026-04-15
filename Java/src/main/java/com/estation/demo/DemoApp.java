package com.estation.demo;

import com.estation.demo.entities.ApHeartbeat;
import com.estation.demo.entities.EStationConfig;
import com.estation.demo.entities.EStationInfo;
import com.estation.demo.entities.EStationMessage;
import com.estation.demo.entities.EslEntity;
import com.estation.demo.entities.EslEntity2;
import com.estation.demo.entities.OtaData;
import com.estation.demo.entities.TaskResult;
import com.estation.demo.enums.PageIndex;
import com.estation.demo.enums.Pattern;
import com.estation.demo.util.HashUtils;
import com.estation.demo.util.ImageData;
import com.estation.demo.util.ImageUtils;
import com.estation.demo.util.MessagePackUtil;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.file.Path;
import java.time.Instant;
import java.util.Base64;
import java.util.List;
import java.util.Random;
import java.util.Scanner;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.zip.GZIPOutputStream;
import org.eclipse.paho.client.mqttv3.IMqttDeliveryToken;
import org.eclipse.paho.client.mqttv3.MqttCallbackExtended;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;

public final class DemoApp {
    private static final AtomicInteger TOKEN = new AtomicInteger(new Random().nextInt(0xFFFF));

    // Demo parameters - these can be modified as needed for testing. 
    // The tag IDs should correspond to actual tags in your environment if you want to see the updates reflected on real devices. 
    // The image paths should point to valid image files on your system.
    private static final String TAG_ID = "82000088A2C8";
    private static final String TAG_ID_2 = "A00000CA8033";
    private static final String TAG_ID_3 = "810000AB114B";

    private static final Path TEST_IMAGE = Path.of("src/main/java/com/estation/demo/images/T1.bmp");
    private static final Path TEST_IMAGE_2 = Path.of("src/main/java/com/estation/demo/images/T2.bmp");
    private static final Path TEST_IMAGE_3_A = Path.of("src/main/java/com/estation/demo/images/T3A.bmp");
    private static final Path TEST_IMAGE_3_B = Path.of("src/main/java/com/estation/demo/images/T3B.bmp");

    // ApConfiguration parameters - adjust these to match your MQTT broker and network settings.
    private static final String ALIAS = "09";
    private static final String SERVER = "192.168.4.74:9071";
    private static final String USER_NAME = "test";
    private static final String PASSWORD = "123456";
    private static final boolean ENCRYPT = false;
    private static final boolean AUTO_IP = false;
    private static final String LOCAL_IP = "192.168.4.101";
    private static final String SUBNET_MASK = "255.255.255.0";
    private static final String GATEWAY = "192.168.4.1";
    private static final int HEARTBEAT = 60;

    private DemoApp() {
    }

    public static void main(String[] args) throws Exception {
        System.out.println("Starting eStation Demo(Java)...");
        System.out.println("0: Publish Config");
        System.out.println("1: Publish ESL");
        System.out.println("2: Publish ESL2");
        System.out.println("3: Publish DSL(TODO)");
        System.out.println("4: Publish DSL2(TODO)");
        System.out.println("5: Publish ESL OTA(TODO)");
        System.out.println("6: Publish Firmware OTA");
        System.out.println("Press 'E' to exit.");
        System.out.println("Sample assets: " + TEST_IMAGE + ", " + TEST_IMAGE_2 + ", " + TEST_IMAGE_3_A + ", " + TEST_IMAGE_3_B + ", " + TAG_ID_3);

        MqttClient client = new MqttClient("tcp://" + AppConfig.BROKER + ":" + AppConfig.PORT, "test_server");
        client.setCallback(new DemoCallback(client));

        MqttConnectOptions options = new MqttConnectOptions();
        options.setAutomaticReconnect(true);
        options.setCleanSession(true);
        options.setUserName(AppConfig.USER_NAME);
        options.setPassword(AppConfig.PASSWORD.toCharArray());
        client.connect(options);

        String otaDownloadUrl = "http://192.168.4.74:9070/ota/2/eStation2.1.0.44.OTA.tar?id=%s&time=%d";
        String otaConfirmUrl = "http://192.168.4.74:9070/confirm?id=%s&time=%d";

        try (Scanner scanner = new Scanner(System.in)) {
            while (true) {
                System.out.print("Enter code to send: ");
                String userInput = scanner.nextLine().trim();
                if (userInput.equalsIgnoreCase("E")) {
                    break;
                }

                try {
                    int code = Integer.parseInt(userInput);
                    switch (code) {
                        case 0 -> publishConfig(client);
                        case 1 -> publishEsl(client, TAG_ID, nextToken(), TEST_IMAGE, true, false, false);
                        case 2 -> publishEsl2(client, TAG_ID_2, nextToken(), TEST_IMAGE_2, false, true, false);
                        case 3 -> System.out.println("DSL publish is not implemented yet.");
                        case 4 -> System.out.println("DSL2 publish is not implemented yet.");
                        case 5 -> System.out.println("ESL OTA publish is not implemented yet.");
                        case 6 -> {
                            long timestamp = Instant.now().getEpochSecond();
                            publishOta(
                                    client,
                                    Path.of("/Users/andersonh/Documents/GitHub/eStation/OTA/AP_OTA/eStation2.1.0.44.OTA.tar"),
                                    "1.0.44",
                                    otaDownloadUrl.formatted("0001", timestamp),
                                    otaConfirmUrl.formatted("0001", timestamp)
                            );
                        }
                        default -> System.out.println("Unknown code.");
                    }
                } catch (NumberFormatException e) {
                    System.out.println("Invalid input. Please enter a valid code.");
                }
            }
        } finally {
            client.disconnect();
            client.close();
        }
    }

    private static void publishConfig(MqttClient client) throws MqttException {
        EStationConfig config = new EStationConfig(
                ALIAS,
                SERVER,
                List.of(USER_NAME, PASSWORD),
                ENCRYPT,
                AUTO_IP,
                LOCAL_IP,
                SUBNET_MASK,
                GATEWAY,
                HEARTBEAT
        );
        client.publish(AppConfig.TOPIC_CONFIG, new MqttMessage(config.toMessagePack()));
    }

    private static void publishEsl(MqttClient client, String tagId, int token, Path imagePath, boolean r, boolean g, boolean b)
            throws IOException, MqttException {
        ImageData image = ImageUtils.readImageBgra(imagePath);
        String base64 = Base64.getEncoder().encodeToString(image.bytes());
        EslEntity entity = new EslEntity(
                tagId,
                Pattern.UPDATE_DISPLAY,
                PageIndex.P0,
                r,
                g,
                b,
                0,
                token,
                "",
                "",
                base64
        );
        byte[] data = MessagePackUtil.pack(List.of(entity.toPayloadList()));
        client.publish(AppConfig.TOPIC_TASK_ESL, new MqttMessage(data));
    }

    private static void publishEsl2(MqttClient client, String tagId, int token, Path imagePath, boolean r, boolean g, boolean b)
            throws IOException, MqttException {
        ImageData image = ImageUtils.readImageBgr(imagePath);
        EslEntity2 entity = new EslEntity2(
                tagId,
                Pattern.UPDATE_DISPLAY,
                PageIndex.P0,
                r,
                g,
                b,
                0,
                token,
                "",
                "",
                gzip(image.bytes()),
                true
        );
        byte[] data = MessagePackUtil.pack(List.of(entity.toPayloadList()));
        client.publish(AppConfig.TOPIC_TASK_ESL2, new MqttMessage(data));
    }

    private static void publishOta(MqttClient client, Path filePath, String version, String downloadUrl, String confirmUrl)
            throws IOException, MqttException {
        OtaData ota = new OtaData(
                downloadUrl,
                confirmUrl,
                0,
                version,
                filePath.getFileName().toString(),
                HashUtils.md5HexUpper(filePath)
        );
        client.publish(AppConfig.TOPIC_FIRMWARE, new MqttMessage(ota.toMessagePack()));
    }

    private static int nextToken() {
        return TOKEN.updateAndGet(current -> current >= 0xFFFF ? 0 : current + 1);
    }

    private static byte[] gzip(byte[] bytes) throws IOException {
        ByteArrayOutputStream buffer = new ByteArrayOutputStream();
        try (GZIPOutputStream gzip = new GZIPOutputStream(buffer)) {
            gzip.write(bytes);
        }
        return buffer.toByteArray();
    }

    private static final class DemoCallback implements MqttCallbackExtended {
        private final MqttClient client;

        private DemoCallback(MqttClient client) {
            this.client = client;
        }

        @Override
        public void connectionLost(Throwable cause) {
            System.out.println("Disconnected from MQTT broker: " + (cause == null ? "unknown cause" : cause.getMessage()));
        }

        @Override
        public void messageArrived(String topic, MqttMessage message) {
            System.out.println("[Recv] Topic: " + topic);
            try {
                switch (topic) {
                    case AppConfig.TOPIC_INFOR -> System.out.println("eStation Infor: " + EStationInfo.fromMessagePack(message.getPayload()));
                    case AppConfig.TOPIC_RESULT -> System.out.println("eStation Result: " + TaskResult.fromMessagePack(message.getPayload()));
                    case AppConfig.TOPIC_HEARTBEAT -> {
                        ApHeartbeat heartbeat = ApHeartbeat.fromMessagePack(message.getPayload());
                        if (heartbeat != null) {
                            // Heartbeats can be noisy; parsed for parity with the Python demo.
                        }
                    }
                    case AppConfig.TOPIC_MESSAGE -> System.out.println("eStation Message: " + EStationMessage.fromMessagePack(message.getPayload()));
                    default -> {
                    }
                }
            } catch (RuntimeException ex) {
                System.err.println("Failed to decode message on " + topic + ": " + ex.getMessage());
            }
        }

        @Override
        public void deliveryComplete(IMqttDeliveryToken token) {
        }

        @Override
        public void connectComplete(boolean reconnect, String serverURI) {
            System.out.println("Connected to MQTT broker " + serverURI + (reconnect ? " (reconnected)" : ""));
            try {
                client.subscribe(AppConfig.TOPIC_INFOR);
                client.subscribe(AppConfig.TOPIC_RESULT);
                client.subscribe(AppConfig.TOPIC_HEARTBEAT);
                client.subscribe(AppConfig.TOPIC_MESSAGE);
            } catch (MqttException e) {
                throw new RuntimeException("Failed to subscribe to topics", e);
            }
        }
    }
}
