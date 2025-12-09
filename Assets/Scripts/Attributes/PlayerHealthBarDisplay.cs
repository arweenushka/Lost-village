using System;
using TMPro;
using UnityEngine;

namespace Attributes
{
    //used if using healthBar instead of text statistic
    public class PlayerHealthBarDisplay : MonoBehaviour
    {
        private Health health;
        [SerializeField] private HealthBar healthBar;
        
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            //issue with set max health method. instead of 100 health  value is 111. defect opened
            //issue with progression as well. when i am starting on 4th level, i see 275/200 and if after take
            //an armour immidiatelly changes to 200/210
            healthBar.SetMaxHealth(health.GetHealthPercentage());
            healthBar.SetHealth(health.GetHealthPercentage());
            
            //new
            DisplayHealth();
        }

        private void Update()
        {
            DisplayHealth();
        }
        
        private void DisplayHealth()
        {
            healthBar.SetHealth(health.GetHealthPercentage());
        }
    }
}
