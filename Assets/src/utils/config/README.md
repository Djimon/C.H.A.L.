# ConfigurationLoader Class Documentation

## Table of Contents
- [Class: ConfigurationLoader](#class-configurationloader)
  - [Properties](#properties)
  - [Methods](#methods)
    - [Awake()](#awake)
    - [Start()](#start)
    - [LoadRarityConfig()](#loadrarityconfig)
    - [GetConfigForRarity(string rarity)](#getconfigforraritystring-rarity)
    - [LoadDebugConfig()](#loaddebugconfig)
    - [LoadRewardsConfig()](#loadrewardsconfig)
    - [SaveRewardsToJson(RewardConfig config)](#saverewardstojsonrewardconfig-config)
    - [CalculateReward(EMonsterType monsterType, EUnitSize monsterSize, string rewardType)](#calculaterewardemonstertype-monstertype-eunitsize-monstersize-string-rewardtype)
    - [OnDisable()](#ondisable)
    - [WriteUpdatedConfig()](#writeupdatedconfig)
- [Class: DebugConfig](#class-debugconfig)
  - [Properties](#properties)
  - [Methods](#methods)
    - [ApplyConfig(string property, string value)](#applyconfigstring-property-string-value)
    - [ApplySettings()](#applysettings)
- [Class: RarityConfigEntry](#class-rarityconfigentry)
- [Class: RarityConfig](#class-rarityconfig)
- [Class: RewardConfig](#class-rewardconfig)
- [Class: BaseRewards](#class-baserewards)
- [Class: RewardModifierEntry](#class-rewardmodifierentry)
- [Class: RewardModifiers](#class-rewardmodifiers)
- [Raw-Classes](#raw-classes)


---

## Class: `ConfigurationLoader`

The `ConfigurationLoader` class is a singleton responsible for loading and managing various game configurations, such as rarity, debug, and reward configurations. It ensures these settings are loaded at runtime and are accessible globally throughout the game.

### Properties

- **`public static ConfigurationLoader Instance { get; private set; }`**  
  A singleton instance of the `ConfigurationLoader` class. Ensures that only one instance exists globally.

- **`private GameManager gameManager`**  
  A reference to the `GameManager` component attached to the same GameObject.

- **`public RarityConfig rarityConfig`**  
  Stores the configuration for item or entity rarity.

- **`public DebugConfig debugConfig`**  
  Stores the debug configuration settings for the game.

- **`public RewardConfig rewardConfig`**  
  Stores the reward configuration for different monsters and actions.

- **`public RawRewardConfig tempRewardConfig`**  
  Temporarily holds the raw reward configuration before it is processed.

---

### Methods

#### Awake()

- **Signature**: `private void Awake()`
- **Description**: Initializes the singleton instance of `ConfigurationLoader`. Loads various configurations on game start and makes the instance persistent across scenes using `DontDestroyOnLoad`.
- **Usage**: This method should be called automatically when the `MonoBehaviour` is instantiated.

#### Start()

- **Signature**: `private void Start()`
- **Description**: Currently, this method is a placeholder and does not execute any specific logic.

#### LoadRarityConfig()

- **Signature**: `public void LoadRarityConfig()`
- **Description**: Loads the rarity configuration from a JSON file located at `Assets/config/rarityConfig.json`. If the file is not found, an error is logged.
- **Usage**: Call this method to initialize the `rarityConfig` object.

#### GetConfigForRarity(string rarity)

- **Signature**: `public RarityConfigEntry GetConfigForRarity(string rarity)`
- **Description**: Retrieves a specific rarity configuration entry based on the provided `rarity` string.
- **Parameters**: 
  - `string rarity`: The rarity identifier.
- **Returns**: The corresponding `RarityConfigEntry` object.

#### LoadDebugConfig()

- **Signature**: `public void LoadDebugConfig()`
- **Description**: Loads the debug configuration from the file located at `Assets/config/debug.config`. Parses the configuration file, applies settings, and initializes the `DebugManager`.
- **Usage**: Automatically applies the debug settings upon loading.

#### LoadRewardsConfig()

- **Signature**: `private void LoadRewardsConfig()`
- **Description**: Loads reward configurations from `Assets/config/RewardConfig.json`. Processes the raw reward data and applies it to the `rewardConfig` structure.
- **Usage**: This method should be called to initialize the reward system with proper modifiers and base rewards.

#### SaveRewardsToJson(RewardConfig config)

- **Signature**: `public void SaveRewardsToJson(RewardConfig config)`
- **Description**: Saves the current `RewardConfig` object into a JSON file at `Assets/config/RewardConfig.json`.
- **Parameters**: 
  - `RewardConfig config`: The reward configuration to save.
- **Usage**: Call this method to persist any changes made to the reward settings.

#### CalculateReward(EMonsterType monsterType, EUnitSize monsterSize, string rewardType)

- **Signature**: `public int CalculateReward(EMonsterType monsterType, EUnitSize monsterSize, string rewardType)`
- **Description**: Calculates the reward for defeating a monster of the specified type and size, based on the reward type (e.g., `Gold`, `XP`, `Crystals`). The reward is adjusted using base values and modifiers.
- **Parameters**: 
  - `EMonsterType monsterType`: The type of monster.
  - `EUnitSize monsterSize`: The size of the monster.
  - `string rewardType`: The type of reward (`Gold`, `XP`, or `Crystals`).
- **Returns**: The calculated reward value as an integer.

#### OnDisable()

- **Signature**: `private void OnDisable()`
- **Description**: Automatically called when the object is disabled. Ensures that any updates to the debug configuration are written to the config file.

#### WriteUpdatedConfig()

- **Signature**: `private void WriteUpdatedConfig()`
- **Description**: Updates the debug configuration file with any changes to excluded tags or settings. Writes the updated settings back to `Assets/config/debug.config`.
- **Usage**: Automatically called when the script is disabled or when configuration updates are necessary.

---

## Class: `DebugConfig`

The `DebugConfig` class manages the configuration for debugging the game. It allows the specification of debug levels, productive modes, and tag-based logging inclusion/exclusion.

### Properties

- **`public DebugManager.EDebugLevel DebugLevel = DebugManager.EDebugLevel.Production`**  
  The current debug level for logging. It is initialized to the `Production` level by default.

- **`public bool ProductiveMode = false`**  
  A flag that indicates whether the game is running in productive mode (true) or debug mode (false).

- **`public HashSet<string> ActiveTags = new HashSet<string>()`**  
  A set of active tags for filtering debug logs. Only logs with these tags will be printed if specified.

- **`public HashSet<string> ExcludedTags = new HashSet<string>()`**  
  A set of tags that should be excluded from the debug logs.

---

### Methods

#### ApplyConfig(string property, string value)

- **Signature**: `public void ApplyConfig(string property, string value)`
- **Description**: Configures the debug settings by applying the provided property and its corresponding value. Handles various properties such as `DebugLevel`, `ProductiveMode`, `ActiveTags`, and `ExcludedTags`.
- **Parameters**: 
  - `string property`: The name of the property to configure (e.g., `DebugLevel`, `ProductiveMode`).
  - `string value`: The value to be applied to the property.
- **Usage**: This method is used to set individual debug settings based on configuration files.

#### ApplySettings()

- **Signature**: `public void ApplySettings()`
- **Description**: Applies the current debug settings to the `DebugManager`. It sets the debug level, productive mode, and activates or deactivates tags based on the configuration.
- **Usage**: This method should be called after loading or modifying the debug configuration to apply the new settings.

---

## Class: `RarityConfigEntry`

The `RarityConfigEntry` class represents a single entry in the rarity configuration. It stores data related to the rarity of items or entities, including price and DNA multipliers.

### Properties

- **`public string rarity`**  
  The name or identifier of the rarity level (e.g., "Common", "Rare", "Epic").

- **`public float priceMultiplier`**  
  The price multiplier associated with this rarity level. It adjusts the base price of items with this rarity.

- **`public float dnaMultiplier`**  
  The DNA multiplier associated with this rarity level. It adjusts the amount of DNA (or equivalent resource) obtained for this rarity.

---

## Class: `RarityConfig`

The `RarityConfig` class holds the full list of rarity configurations in the game. Each rarity configuration is represented by a `RarityConfigEntry`.

### Properties

- **`public List<RarityConfigEntry> Rarities`**  
  A list of `RarityConfigEntry` objects, each representing a different rarity level (e.g., Common, Rare, etc.) with its corresponding multipliers for price and DNA.

---

## Class: `RewardConfig`

Represents the overall reward configuration, containing base rewards and reward modifiers.

### Properties
- **`BaseRewards baseRewards`**: The base reward values for Gold, XP, and Crystals.
- **`List<RewardModifierEntry> rewardModifiers`**: A list of reward modifier entries, each corresponding to a specific monster type.

---

## Class: `BaseRewards`

Stores the base reward values for different reward types.

### Properties
- **`int Gold`**: The base reward value for Gold.
- **`int XP`**: The base reward value for XP.
- **`int Crystals`**: The base reward value for Crystals.

---

## Class: `RewardModifierEntry`

Stores the reward modifiers for a specific monster type.

### Properties
- **`string monsterType`**: The monster type (e.g., "Chitinoid").
- **`List<RewardModifiers> modifiers`**: A list of reward modifiers based on monster size.

---

## Class: `RewardModifiers`

Stores the reward modification values for a specific monster size.

### Properties
- **`string unitSize`**: The size of the monster (e.g., "small", "medium").
- **`float Gold`**: The Gold reward modifier.
- **`float XP`**: The XP reward modifier.
- **`float Crystals`**: The Crystals reward modifier.

---

## Raw-Classes

These classes are build to fnciton as helper to deserialize the JSON-file.

- **`RawRewardConfig`**: Stores the raw reward configuration before it is processed.
- **`RawRewardModifiers`**: Represents the raw configuration for reward modifiers, containing data for each monster type and its corresponding sizes.
- **`RawRewardSizes`**: Represents the raw size configuration for monsters and their corresponding rewards.
- **`RawRewards`**: Represents the raw reward values interpreted as percentages.