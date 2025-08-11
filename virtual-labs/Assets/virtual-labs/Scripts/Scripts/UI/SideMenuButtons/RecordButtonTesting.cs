using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table.UI.SideMenuButtons
{
    public class RecordButtonTesting : MonoBehaviour
    {
        [SerializeField] List<CellData> cellsData;
        [SerializeField] private RecordReadingsButton _recordRedingsButton;
        public void Randomize()
        {
            if (cellsData == null)
                cellsData = new List<CellData>();
            cellsData.Clear();
            cellsData.Add(new() { FieldName = "Name", Value = RandomName() });
            cellsData.Add(new() { FieldName = "Number", Value = RandomAge() });
            cellsData.Add(new() { FieldName = "Smoker", Value = RandomSmoke() });
            cellsData.Add(new() { FieldName = "Grade", Value = RandomGrade() });
            _recordRedingsButton.CellsData = cellsData;
        }

        public string RandomName()
        {
            return (Random.Range(0, 10)) switch
            {
                0 => "Ahmed",
                1 => "Mohamed",
                2 => "Amr",
                3 => "Mazen",
                4 => "Karim",
                5 => "Gamal",
                6 => "Osama",
                7 => "Abdallah",
                8 => "Esam",
                9 => "Esmat",
                10 => "Mahmoud",
                _ =>"Bondo2"
            };
        }

        public string RandomAge()
        {
            return (Random.Range(0, 10)) switch
            {
                0 => "20",
                1 => "21",
                2 => "22",
                3 => "23",
                4 => "24",
                5 => "25",
                6 => "26",
                7 => "27",
                8 => "28",
                9 => "29",
               10 => "30",
                _ => "89"
            };
        }

        public string RandomSmoke()
        {
            return (Random.Range(0, 2)) switch
            {
                0 => "true",
                1 => "false",
                _ => "false"
            };
        }

        public string RandomGrade()
        {
            return (Random.Range(0, 4)) switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "F",
                4 => "VeryVery big text option",
                _ => "Bondo2"
            };
        }
    }
}
