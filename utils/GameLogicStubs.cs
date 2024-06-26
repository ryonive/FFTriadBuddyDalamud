﻿using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;

// also know as: i should've made gamelogic into a library
//
// implements stubs for stuff used by gamelogic classes

namespace MgAl2O4.Utils
{
    public class Logger
    {
#if DEBUG
        public static IPluginLog? logger;
#endif // DEBUG

        public static void WriteLine(string fmt, params object[] args)
        {
#if DEBUG
            logger?.Info(string.Format(fmt, args));
#endif // DEBUG
        }
    }
}

namespace FFTriadBuddy
{
    public enum ELocStringType
    {
        Unknown,
        RuleName,
        CardType,
        CardName,
        NpcName,
    }

    public class LocString
    {
        public string Text = string.Empty;
        public ELocStringType Type;
        public int Id;

        public LocString(ELocStringType Type, int Id)
        {
            this.Type = Type;
            this.Id = Id;
        }

        public LocString(ELocStringType Type, int Id, string DefaultText)
        {
            this.Type = Type;
            this.Id = Id;
            Text = DefaultText;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1} '{2}'", Type, Id, GetCodeName());
        }

        public string GetLocalized()
        {
            // nope
            return Text;
        }

        public string GetCodeName()
        {
            // nope
            return Text;
        }
    }

    public class LocalizationDB
    {
        public readonly static string[] Languages = { "de", "en", "fr", "ja", "cn", "ko" };
        public static int UserLanguageIdx = 1;
        private static LocalizationDB instance = new LocalizationDB();

        public List<LocString> LocUnknown = new List<LocString>();
        public List<LocString> LocRuleNames = new List<LocString>();
        public List<LocString> LocCardTypes = new List<LocString>();
        public List<LocString> LocCardNames = new List<LocString>();
        public List<LocString> LocNpcNames = new List<LocString>();

        public Dictionary<ELocStringType, List<LocString>> mapLocStrings;
        public Dictionary<ETriadCardType, LocString> mapCardTypes;

        public static LocalizationDB Get()
        {
            return instance;
        }

        public LocalizationDB()
        {
            mapLocStrings = new Dictionary<ELocStringType, List<LocString>>();
            mapLocStrings.Add(ELocStringType.Unknown, LocUnknown);
            mapLocStrings.Add(ELocStringType.RuleName, LocRuleNames);
            mapLocStrings.Add(ELocStringType.CardType, LocCardTypes);
            mapLocStrings.Add(ELocStringType.CardName, LocCardNames);
            mapLocStrings.Add(ELocStringType.NpcName, LocNpcNames);

            mapCardTypes = new Dictionary<ETriadCardType, LocString>();
            string[] enumNames = Enum.GetNames(typeof(ETriadCardType));
            for (int enumIdx = 0; enumIdx < enumNames.Length; enumIdx++)
            {
                var locStr = new LocString(ELocStringType.CardType, enumIdx, enumIdx == 0 ? "" : enumNames[enumIdx]);
                mapCardTypes.Add((ETriadCardType)enumIdx, locStr);
                LocCardTypes.Add(locStr);
            }
        }

        public LocString FindOrAddLocString(ELocStringType Type, int Id)
        {
            var list = mapLocStrings[Type];

            // so far those ids are continuous from 0, switch to dictionaries when it changes
            while (list.Count <= Id)
            {
                list.Add(new LocString(Type, list.Count));
            }

            return list[Id];
        }
    }

    public class PlayerSettingsDB
    {
        private static PlayerSettingsDB instance = new PlayerSettingsDB();
        public List<TriadCard> ownedCards = new List<TriadCard>();
        public TriadCard?[] starterCards;

        public static PlayerSettingsDB Get()
        {
            return instance;
        }

        public PlayerSettingsDB()
        {
            TriadCardDB cardDB = TriadCardDB.Get();
            starterCards = new TriadCard?[5];

            // localization is not fully loaded, just active language!
            // find starter cards by their ids 
            starterCards[0] = cardDB.FindById(1); // Dodo
            starterCards[1] = cardDB.FindById(3); // Sabotender
            starterCards[2] = cardDB.FindById(6); // Bomb
            starterCards[3] = cardDB.FindById(7); // Mandragora
            starterCards[4] = cardDB.FindById(10); // Coeurl
        }
    }

    public class ScannerTriad
    {
        public enum ETurnState
        {
            MissingTimer,
            Waiting,
            Active,
        }

        public class GameState
        {
            public TriadCard?[] board = new TriadCard[9];
            public ETriadCardOwner[] boardOwner = new ETriadCardOwner[9];
            public TriadCard?[] blueDeck = new TriadCard[5];
            public TriadCard?[] redDeck = new TriadCard[5];
            public TriadCard? forcedBlueCard = null;
            public List<TriadGameModifier> mods = new List<TriadGameModifier>();
            public ETurnState turnState = ETurnState.MissingTimer;
        }
    }
}
