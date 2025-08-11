
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

namespace Praxilabs.UIs
{
    public class SampleHintMessage : MonoBehaviour
    {
        [SerializeField] private HintsManager hintsManager;
        //[SerializeField] private HintsDataScriptable HintsDataScriptable;
        
        //private HintsDataScriptable _hintsData;
        
        [SerializeField] private int _stepNumberToOpenWith;
        //[SerializeField] private CanvasWebViewPrefab _canvasWebViewPrefab;
        //private void Start()
        //{
        //    _hintsData = ScriptableObject.CreateInstance<HintsDataScriptable>();
        //    _hintsData.StepsData.Add(new HintsList
        //    {
        //        Hints = new List<string> { "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">1 : Hint for step 1!!</span>\n</h2>\n" +
        //                                   "<h4 style=\"text-align:center; color:#595379;\">\n " +
        //                                   "<strong> 1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!:(</strong><br>\n " +
        //                                   "</p>" ,
        //                                   "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">2 : Hint for step 1!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                   "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                   "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" },
                
        //        TeaseHints = new List<string> { },
        //        State = StateTypes.Unfinished  
        //    });

        //    _hintsData.StepsData.Add(new HintsList
        //    {
        //        Hints = new List<string> { "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">1 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                   "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                   "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">2 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">3 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">4 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>"  },
                
                
                
                
        //        TeaseHints = new List<string> { "<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                        "<strong>1 : Tease Hint for step 2</strong><br>\n    ",
        //                                        "<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                        "<strong>2 : Tease Hint for step 2</strong>\n</h4>"
        //        },
                
        //        State = StateTypes.Unfinished  
        //    });  _hintsData.StepsData.Add(new HintsList
        //    {
        //        Hints = new List<string> { "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">1 : Hint for step 1!!</span>\n</h2>\n" +
        //                                   "<h4 style=\"text-align:center; color:#595379;\">\n " +
        //                                   "<strong> 1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!:(</strong><br>\n " +
        //                                   "</p>" ,
        //                                   "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">2 : Hint for step 1!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                   "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                   "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" },
                
        //        TeaseHints = new List<string> { },
        //        State = StateTypes.Current  
        //    });

        //    _hintsData.StepsData.Add(new HintsList
        //    {
        //        Hints = new List<string> { "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">1 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                   "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                   "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">2 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">3 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">4 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>"  },
                
                
                
                
        //        TeaseHints = new List<string> { "<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                        "<strong>1 : Tease Hint for step 2</strong><br>\n    ",
        //                                        "<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                        "<strong>2 : Tease Hint for step 2</strong>\n</h4>"
        //        },
                
        //        State = StateTypes.Unfinished  
        //    });  _hintsData.StepsData.Add(new HintsList
        //    {
        //        Hints = new List<string> { "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">1 : Hint for step 1!!</span>\n</h2>\n" +
        //                                   "<h4 style=\"text-align:center; color:#595379;\">\n " +
        //                                   "<strong> 1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!1 - Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab, " +
        //                                   "take caution not to do that! Enabling the power supply before configuring its settings can lead to catastrophic problems in the lab," +
        //                                   " take caution not to do that!:(</strong><br>\n " +
        //                                   "</p>" ,
        //                                   "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">2 : Hint for step 1!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                   "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                   "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" },
                
        //        TeaseHints = new List<string> { },
        //        State = StateTypes.Finished  
        //    });

        //    _hintsData.StepsData.Add(new HintsList
        //    {
        //        Hints = new List<string> { "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                   " <span style=\"color:hsl(357,97%,71%);\">1 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                   "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                   "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">2 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">3 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>" ,
        //                                    "<p>\n<h2 style=\"text-align:center;\">\n   " +
        //                                    " <span style=\"color:hsl(357,97%,71%);\">4 : Hint for step 2!!</span>\n</h2>\n<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                    "<strong>Your time for this experiment is out! unfortunately you can’t complete your progress at this moment :(</strong><br>\n    " +
        //                                    "<strong>Remember next time to be able to manage your time more efficiently! &nbsp;</strong>\n</h4>\n</p>"  },
                
                
                
                
        //        TeaseHints = new List<string> { "<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                        "<strong>1 : Tease Hint for step 2</strong><br>\n    ",
        //                                        "<h4 style=\"text-align:center; color:#595379;\">\n    " +
        //                                        "<strong>2 : Tease Hint for step 2</strong>\n</h4>"
        //        },
                
        //        State = StateTypes.Finished  
        //    });
        //}
        public void UpdateStepState(int step , StateTypes state )
        {
            
        }
        public void OpenHints()
        {
            //hintsManager.OpenHintsMenu(_hintsData, _stepNumberToOpenWith);
            //hintsManager.Open(HintsDataScriptable, _stepNumberToOpenWith);
        }
    }
}
