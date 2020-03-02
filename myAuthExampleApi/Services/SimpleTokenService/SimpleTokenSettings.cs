namespace Services.SimpleTokenServ
{
    public class SimpleTokenSettings
    {
        public int TokenExpirationInMinutes { get; set; } = 1440; //default 1440 = 1 day
        public int CooldownPeriodInMinutes { get; set; } = 5; //A user may not create a new simpleToken within the cooldown period. This helps to prevent api misuse (like spamming)
    }
}
