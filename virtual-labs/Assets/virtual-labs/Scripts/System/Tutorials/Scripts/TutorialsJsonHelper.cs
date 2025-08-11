using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialsJsonHelper : MonoBehaviour
{
    [SerializeField] TutorialSO _tutorialSO;
    [SerializeField] private TutorialsManager _tutorialsManager;

    string json = @"
        {
            ""_tutorialModules"": [
                {
                    ""_title"": ""Exploring Interfaces"",
                    ""Status"": ""UnFinished"",
                    ""_data"": [
                        {
                            ""_title"": ""Lab Controls"",
                            ""_info"": ""Use the lab controls to move around the virtual lab. You can easily explore different areas and interact with key points to make sure you're completing each part of the experiment.""
                        },
 {
                            ""_title"": ""Oxi"",
                            ""_info"": ""Oxi is your helpful lab companion! It provides clear instructions and hints, offering feedback along the way. Helping you understand if you’re on the right track or if something needs adjustment.""
                        },
 {
                            ""_title"": ""Settings"",
                            ""_info"": ""The settings menu lets you adjust experiment options like sound and blinking to suit your preferences. You can also access helpful documents to guide you through the experiment, and easily restart or exit if needed.""
                        },
 {
                            ""_title"": ""Stages/Progress bar"",
                            ""_info"": ""The progress bar keeps you informed about your progress in both the current stage and the entire experiment. It gives you a quick visual of how far you’ve come and how much is left to do.""
                        },
 {
                            ""_title"": ""Side elements"",
                            ""_info"": ""The side menu is your tool hub, where you can find a stopwatch to track time accurately, a button to record your data, and a table to view and manage everything you've recorded throughout the experiment.""
                        }
                    ]
                },
{
                    ""_title"": ""Settings"",
                    ""Status"": ""UnFinished"",
                    ""_data"": [
                        {
                            ""_title"": ""Open the Settings Menu"",
                            ""_info"": ""Click the gear icon to access:  Lab Manual: Experiment guide. Enrichment Materials: Extra learning resources. Animated Video: Visual overview. Walkthrough: Step-by-step help.""
                        },
                    ]
                },


{
                    ""_title"": ""Lab Controls"",
                    ""Status"": ""UnFinished"",
                    ""_data"": [
                        {
                            ""_title"": ""Default View"",
                            ""_info"": ""Click the Default View button to reset the camera.""
                        },
 {
                            ""_title"": ""Camera Movement:"",
                            ""_info"": ""Use the right and left arrows to move the camera.""
                        },
 {
                            ""_title"": ""Zoom Controls"",
                            ""_info"": ""Use the plus (+) and minus (-) signs to zoom in or out.""
                        },
 {
                            ""_title"": ""Wide Lab View"",
                            ""_info"": ""Click the man icon to access a wide lab view with points of interest.""
                        },
 {
                            ""_title"": ""Free Movement"",
                            ""_info"": ""Move forward, backward, left, or right using the arrows around the Default View button.""
                        },
 {
                            ""_title"": ""Rotating the View"",
                            ""_info"": ""Hold right-click and drag to freely rotate the view.""
                        }
                    ]
                },
{
                    ""_title"": ""Hints"",
                    ""Status"": ""UnFinished"",
                    ""_data"": [
                        {
                            ""_title"": ""Access Instructions and Hints"",
                            ""_info"": ""Click on Oxi for step-specific instructions and hints.""
                        },
 {
                            ""_title"": ""View All Steps"",
                            ""_info"": ""Click All Steps to view a list of steps and hints. Select a step to navigate directly to it.""
                        },
 {
                            ""_title"": ""Hints Checkpoints"",
                            ""_info"": ""Completed steps: solid dark purple circle with a checkmark. Current step: dotted circle. Upcoming steps: empty circles. Click any step to navigate directly.""
                        },
 {
                            ""_title"": ""Next and Back Buttons"",
                            ""_info"": ""Use Next and Back buttons to navigate between steps.""
                        },

                    ]
                },
{
                    ""_title"": ""Stages Progress"",
                    ""Status"": ""UnFinished"",
                    ""_data"": [
                        {
                            ""_title"": ""Circular Progress Bar"",
                            ""_info"": ""The Progress Circle in the upper-right corner dynamically tracks your experiment advancement.""
                        },
                        {
                            ""_title"": ""Orange Ring"",
                            ""_info"": ""The Orange Ring shows progress within the current stage.""
                        },
  {
                            ""_title"": ""Indigo Ring"",
                            ""_info"": ""The Indigo Ring displays your total progress across all stages.""
                        },
  {
                            ""_title"": ""Detailed Percentages"",
                            ""_info"": ""Hover over the center ratio to view exact percentages for the current stage and overall progress.""
                        },
  {
                            ""_title"": ""Expanded View"",
                            ""_info"": ""Click the Progress Bar to see a breakdown of stages and substages.""
                        },
  {
                            ""_title"": ""Completed Stages"",
                            ""_info"": ""Completed Stages are marked with a Checkmark.""
                        },
  {
                            ""_title"": ""Current Stage"",
                            ""_info"": ""Current Stage is highlighted with a Dotted Circle""
                        },
  {
                            ""_title"": ""Upcoming Stages"",
                            ""_info"": ""Upcoming Stages are shown as Empty Circles.""
                        },
  {
                            ""_title"": ""Substages"",
                            ""_info"": ""Use the Substages Checkmarks Bar for detailed substage tracking""
                        },
  {
                            ""_title"": ""Jump to button"",
                            ""_info"": ""Select a stage using the Jump To Button and click to revisit or skip ahead.""
                        }
                    ]
                },

{
                    ""_title"": ""Table"",
                    ""Status"": ""UnFinished"",
                    ""_data"": [
                        {
                            ""_title"": ""Opening the Table"",
                            ""_info"": ""Click the Note Icon in the side menu to access the table.""
                        },
                        {
                            ""_title"": ""Recording Data"",
                            ""_info"": ""Click the Record Button to log readings directly into the table.""
                        },
  {
                            ""_title"": ""Options Menu Download"",
                            ""_info"": ""Click the Download Icon to export the table as an EXCEL file.""
                        },
  {
                            ""_title"": ""Editing"",
                            ""_info"": ""Hover over a recorded cell to reveal the Edit Icon. Click to modify the values.""
                        },
  {
                            ""_title"": ""Deleting"",
                            ""_info"": ""Hover over a row number to reveal the Recycle Bin Icon. Click to delete the row.""
                        },
  {
                            ""_title"": ""Creating New Rows"",
                            ""_info"": ""Click the New Row Button at the bottom of the table to create empty rows for more data.""
                        },
  {
                            ""_title"": ""Adding a new Table"",
                            ""_info"": ""Click the Add Trial Button to create a new table for recording data.""
                        },
  {
                            ""_title"": ""Accessing Tables"",
                            ""_info"": ""Access other tables by clicking their name or icon in the side menu.""
                        },
  {
                            ""_title"": ""Downloading Tables"",
                            ""_info"": ""Use each table's Download Excel Button to save it as an excel file.""
                        }
                    ]
                },

            ]
        }";


    private void Start()
    {
        TutorialSO tutorials = JsonConvert.DeserializeObject<TutorialSO>(json);

        foreach (TutorialObjectData tutorialModule in tutorials._tutorialModules)
        {
            TutorialObjectData tmpModule = _tutorialSO._tutorialModules.Find(tmpModule => tmpModule._title == tutorialModule._title);

            foreach (TutorialData innerData in tutorialModule._data)
            {
                TutorialData tmpInnerData = tmpModule._data.Find(tmpInnerData => tmpInnerData._title == innerData._title);

                innerData._cardPositionPlaceholder = tmpInnerData._cardPositionPlaceholder;
                innerData._cardAnchorMin = tmpInnerData._cardAnchorMin;
                innerData._cardAnchorMax = tmpInnerData._cardAnchorMax;
                innerData._imageData = tmpInnerData._imageData;
                innerData._animationData = tmpInnerData._animationData;
            }

        }
        _tutorialsManager.InitializeTutorials(tutorials);
    }
}


