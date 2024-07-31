using System.Linq;

public static class PowerUpsInInventory
{
    public enum PowerUpType
    {
        MovementSpeedBoost,
        CasinoLuckBoost,
        EnergyDepletionReduction,
        HungerDepletionReduction,
    }

    public static bool HasPowerUp(PowerUpType powerUpType)
    {
        var powerUps = PlayerInventory.Instance.GetPowerUps();
        return powerUps.Any(p => p.powerUpType == powerUpType);
    }
}
