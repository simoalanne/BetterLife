using System;
using System.Collections.Generic;
using Casino;
using Casino.BlackJack;
using Casino.Roulette;
using Casino.Slots;
using DialogueSystem;
using Player;
using UI;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Services
{
    private static readonly Dictionary<Type, (object Service, bool DontDestroy)> ServiceLookup = new();

    /// <summary>
    /// Tries to register a service. Destroys existing service if it's not marked as persistent.
    /// </summary>
    /// <param name="service"> The service instance to register</param>
    /// <param name="persistent"> If true, the service GameObject will not be replaced when another instance tries to register</param>
    /// <typeparam name="T"> The type of the service, must be a MonoBehaviour</typeparam>
    /// <returns> True if the service was registered, false if there is already a persistent instance</returns>
    public static bool Register<T>(T service, bool persistent = false) where T : MonoBehaviour
    {
        var type = service.GetType();
        if (ServiceLookup.TryGetValue(type, out var existing) && existing.DontDestroy)
        {
            Object.Destroy(service.gameObject);
            return false;
        }

        ServiceLookup[type] = (service, persistent);

        foreach (var iFace in type.GetInterfaces())
            ServiceLookup[iFace] = (service, persistent);
        
        if (persistent && service.transform.parent == null)
        {
            Object.DontDestroyOnLoad(service.gameObject);
        }

        return true;
    }

    public static T Get<T>() where T : class =>
        ServiceLookup.TryGetValue(typeof(T), out var tuple) && tuple.Service is T service
            ? service
            : throw new Exception($"Service {typeof(T)} not found");

    public static bool TryGet<T>(out T service) where T : class
    {
        if (ServiceLookup.TryGetValue(typeof(T), out var tuple) && tuple.Service is T foundService)
        {
            service = foundService;
            return true;
        }

        service = null;
        return false;
    }


    public static T TryGet<T>() where T : class
    {
        return ServiceLookup.TryGetValue(typeof(T), out var tuple) && tuple.Service is T foundService
            ? foundService
            : null;
    }

    // Convenience properties. Some will return null some will throw
    public static BetSizeManager BetSizeManager => Get<BetSizeManager>();
    public static ICasinoMoneyHandler MoneyHandler => Get<ICasinoMoneyHandler>();

    public static SlotMachineMoneyHandler SlotMoneyHandler => Get<SlotMachineMoneyHandler>();

    public static BlackjackMoneyHandler BlackjackMoneyHandler => Get<BlackjackMoneyHandler>();
    public static BlackjackDeck BlackjackDeck => Get<BlackjackDeck>();
    public static BlackjackManager BlackjackManager => Get<BlackjackManager>();
    public static ChipPlacer ChipPlacer => Get<ChipPlacer>();

    public static RouletteMoneyHandler RouletteMoneyHandler => Get<RouletteMoneyHandler>();
    public static RouletteSpinner RouletteSpinner => Get<RouletteSpinner>();
    public static ButtonHighlight ButtonHighlight => Get<ButtonHighlight>();
    public static TooltipDisplayer TooltipDisplayer => Get<TooltipDisplayer>();

    public static DialogueHandler DialogueHandler => Get<DialogueHandler>();

    public static InputManager InputManager => Get<InputManager>();
    public static PlayerManager PlayerManager => Get<PlayerManager>();
    public static PlayerHUD PlayerHUD => Get<PlayerHUD>();
    public static SceneLoader SceneLoader => Get<SceneLoader>();
    public static GameTimer GameTimer => Get<GameTimer>();
    public static PauseMenu PauseMenu => Get<PauseMenu>();
}
