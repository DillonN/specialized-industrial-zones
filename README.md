# Specialized Industrial Zones

Adds new industrial zoning options to the game that are restricted to specific industrial resources.
These zones will spawn the same buildings you're used to from the general industrial zone,
but they are filtered to only spawn buildings that produce the resource of the zone.

**NOTE**: The production buildings are mostly the same across various resources, as they are capable of
producing any resource. However, storage buildings are mostly distinct for the raw resources they store
so you will never e.g. see a wood pile spawn in an oil zone.

# Zone Types
Currently there are four new zone types, one for each of the base game resources:
* Agriculture
  * Grain
  * Livestock
  * Vegetables
  * Cotton
  * Beverages
  * Convenience Food
  * Food
  * Textiles
* Forestry
  * Wood
  * Timber
  * Paper
  * Furniture
* Oil
  * Oil
  * Petrochemicals
  * Chemicals
  * Pharmaceuticals
  * Plastics
  * Electronics
* Ore
  * Ore
  * Coal
  * Stone
  * Minerals
  * Concrete
  * Steel
  * Metals
  * Machinery
  * Vehicles

# Compatibility
This mod is implemented by adding net-new zone prefabs to the game, and copying the base game's set of industrial buildings
to attach to each zone with filters applied. Since it is only adding new content and re-using all the vanilla systems for spawning,
it should be compatible with any mod that doesn't disable the vanilla industrial system.
