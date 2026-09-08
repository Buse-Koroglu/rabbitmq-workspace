# RabbitMQ Exchange Types

An overview of core RabbitMQ exchange types, routing strategies, and messaging patterns.

<img width="1206" height="736" alt="0_gFwb04MsfqtVB5bY" src="https://github.com/user-attachments/assets/8a0342a6-37d1-4aa6-9683-9190185a8f9f" />

---

## 1. Default (Nameless) Exchange

The **Default Exchange** is a pre-declared direct exchange with no explicit name (identified by an empty string `""`). In RabbitMQ, a publisher never connects directly to a queue; it always sends messages to an exchange. 

* **Routing Behavior:** RabbitMQ automatically routes the message to the queue whose name exactly matches the message's `routingKey`.

---

## 2. Fanout Exchange

A **Fanout Exchange** routes incoming messages to **all queues bound to it**, completely ignoring the routing key.

* **Broadcast Behavior:** If three queues are bound to this exchange, a copy of the published message is delivered to all three queues concurrently.
* **Common Pattern (Pub/Sub):** The publisher declares and pushes to the fanout exchange. Each consumer creates its own temporary, uniquely named exclusive queue (e.g., `amq.gen-abc123`), binds it to the exchange, and consumes from it. This ensures every consumer receives an identical copy of every message.
<img width="448" height="448" alt="Fanout-Exchange-in-Rabbit-MQ" src="https://github.com/user-attachments/assets/e94f056b-b205-4ee4-bb21-00358192eba9" />

---

## 3. Direct Exchange

A **Direct Exchange** routes messages to queues based strictly on an exact match between the message's `routingKey` and the queue's binding key.

* **Different Queues with the Same Routing Key (Pub/Sub):** If Consumer 1 and Consumer 2 declare separate queues and both bind them with the same routing key (e.g., `warning`), the exchange duplicates the message and delivers a copy to each queue. Both consumers see every message.
* **Shared Single Queue (Work Queue / Load Balancing):** If both Consumer 1 and Consumer 2 listen to the **same named queue** (e.g., `direct-queue-Warning`), the exchange places only one copy of the message into that queue. Messages are then distributed (round-robin or fair-dispatch) between the consumers so that each message is processed by only one worker.
<img width="648" height="682" alt="Direct-Exchange-in-RabbitMQ" src="https://github.com/user-attachments/assets/34a37cdd-1483-4930-a781-172b9ee987d7" />

---

## 4. Topic Exchange

A **Topic Exchange** routes messages based on wildcard pattern matching between the message's routing key and the binding patterns defined by the queues. Routing keys consist of words separated by dots (e.g., `kern.critical` or `Info.Warning.Error`).

* **Wildcards:**
  * `*` (star): Replaces exactly **one** word.
  * `#` (hash): Replaces **zero or more** words.
* **Example Scenario:**
  * The publisher emits messages using a 3-tier routing key like `${logLevel1}.${logLevel2}.${logLevel3}` (e.g., `Info.Debug.Warning`).
  * If a consumer binds its queue using the pattern `Info.#`, it receives any message starting with `Info`, regardless of the remaining segments (e.g., `Info.Debug` or `Info.Warning.Error`). Messages like `Debug.Info.Warning` will not match and are ignored.
<img width="448" height="492" alt="Topic-Exchange-in-Rabbit-MQ" src="https://github.com/user-attachments/assets/9cd2d1a8-4ad3-4d28-af54-e94a614512ec" />

---

## 5. Headers Exchange

A **Headers Exchange** routes messages based on message metadata (**headers**) instead of routing keys, enabling multi-attribute matching criteria.

* **Matching Strategy (`x-match`):** The queue defines its binding arguments using the `x-match` parameter:
  * `x-match: all`: All specified header key-value pairs must match between the message and the queue binding (default behavior).
  * `x-match: any`: The message is delivered if at least one header key-value pair matches.
* **Case Sensitivity:** Header keys and values are strictly case-sensitive. The `routingKey` parameter is typically left empty (`string.Empty`) when publishing.
<img width="448" height="498" alt="Headers-Exchange-in-Rabbit-MQ" src="https://github.com/user-attachments/assets/03392a33-7e2e-4fe2-b77d-2252027c70e8" />
