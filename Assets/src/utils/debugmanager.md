# DebugManager Class Documentation

## Overview
The `DebugManager` class provides a global mechanism to handle logging messages in different debug levels for an application. It allows logging of system messages, errors, and warnings based on various tags and levels, enabling developers to filter logs according to the current mode (Production, Test, or Debug). The class also supports customizing the color of the log output based on predefined tags or custom settings.

---

## Enum: `EDebugLevel`
Defines different levels of debugging, which determine the verbosity of the logs.

### Values:
- **`Production = 1`**: Logs essential production-level messages.
- **`Test = 2`**: Logs messages intended for testing environments.
- **`Debug = 3`**: Logs detailed debug information for development purposes.

---

## Public Fields

### `CurrentDebugLevel`
- **Type**: `EDebugLevel`
- **Description**: Defines the current global debug level. Logs are displayed only if their level is less than or equal to this value.
- **Default Value**: `EDebugLevel.Production`

### `ProductiveMode`
- **Type**: `bool`
- **Description**: A flag indicating whether the system is running in productive mode. If `true`, only production-level messages will be logged.
- **Default Value**: `false`

### `ActiveTags`
- **Type**: `HashSet<string>`
- **Description**: A collection of active tags that control which messages are logged based on their associated tags.

### `ExcludedTags`
- **Type**: `HashSet<string>`
- **Description**: A collection of tags that are excluded from logging. Messages associated with these tags will not be logged.

---

## Private Fields

### `tagColors`
- **Type**: `Dictionary<string, Color>`
- **Description**: A dictionary mapping specific tags to associated colors. Used for customizing log output based on the tag.
- **Default Values**:
  - `"System"`: `Color.yellow`
  - `"PlayerInfo"`: `Color.blue`
  - `"EventSystem"`: `Color.green`

---

## Public Methods

### `Log(string message, int level = 3, string tag = "System", Color? customColor = null)`
Logs a message based on the current debug level and tag.

#### Parameters:
- **`message`**: The message to log.
- **`level`**: The level of the message (`1` = Production, `2` = Test, `3` = Debug). Defaults to `3` (Debug).
- **`tag`**: The tag used to categorize the message. Defaults to `"System"`.
- **`customColor`** *(optional)*: Custom color for the message log. Defaults to the tag’s predefined color.

#### Behavior:
- Converts the integer `level` to an `EDebugLevel`.
- Logs messages if the current level is less than or equal to `CurrentDebugLevel` and the tag is either empty or in `ActiveTags`.
- Adds the tag to `ExcludedTags` if it is not active.

### `Error(string message, int level = 1, string tag = "System", Color? customColor = null)`
Logs an error message.

#### Parameters:
- Same as `Log()` method.

#### Behavior:
- Functions similarly to `Log()` but logs the message using `Debug.LogError`.

### `Warning(string message, int level = 2, string tag = "System", Color? customColor = null)`
Logs a warning message.

#### Parameters:
- Same as `Log()` method.

#### Behavior:
- Functions similarly to `Log()` but logs the message using `Debug.LogWarning`.

---

## Helper Methods

### `GetGameTime()`
Returns the current game time formatted as a string.

#### Returns:
- **`string`**: The current time in seconds since the game started, with three decimal places.

### `GetTagColor(string tag)`
Retrieves the color associated with a tag.

#### Parameters:
- **`tag`**: The tag for which to retrieve the color.

#### Returns:
- **`Color`**: The color associated with the tag, or white if the tag is not found.

---

## Tag Management

### `SetTagActive(string tag, bool isActive)`
Activates or deactivates a specific tag.

#### Parameters:
- **`tag`**: The tag to activate or deactivate.
- **`isActive`**: If `true`, the tag is activated and added to `ActiveTags`; otherwise, it is deactivated and added to `ExcludedTags`.

---

## Configuration Methods

### `SetDebugLevel(EDebugLevel level)`
Sets the current global debug level.

#### Parameters:
- **`level`**: The new `EDebugLevel` to set.

### `SetProductiveMode(bool productive)`
Enables or disables productive mode.

#### Parameters:
- **`productive`**: If `true`, productive mode is enabled, and only production-level messages are logged.
