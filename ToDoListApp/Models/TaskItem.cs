using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }                 //Identyfikator
        public string Title { get; set; }           //Nazwa zadania
        public string Description { get; set; }     //Opis zadania
        public string Priority { get; set; }        //Priorytet
        public DateTime DueDate { get; set; }       //Termin wykonania
        public bool IsDone { get; set; }            //Status
    }                                               
}                                                   
                                                    