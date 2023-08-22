using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tests : MonoBehaviour
{
   
    [Test]
    public void ValidateRuneSetting()
    {
        ItemDatabase _database = Resources.Load<ItemDatabase>("Database/ItemDatabase");
        
        foreach(BaseItem item in _database.ItemList)
        {
            Rune rune = item as Rune;
            if(rune)
            {
                if(rune.DisplayName == "B등급 공치형 룬")
                {
                   
                    foreach (StatModifier stat in rune.StatModifiers)
                    {
                        
                        if (stat.CodeName == Define.StatID.Attack.ToString())
                        {
                            Assert.AreEqual(stat.Value, 25);
                        }
                        else if(stat.CodeName == Define.StatID.CriticalHit.ToString())
                        {
                            Assert.AreEqual(stat.Value, 75);
                        }
                        else
                        {
                            Assert.AreEqual(true, false);
                        }
                    }
                }
            }
        }
    }

    [Test]
    public void ValidateTrollStageMonsterSpawner()
    {
        MonsterStatCalculator _trollStageCalculator;
        _trollStageCalculator = new MonsterStatCalculator();

        _trollStageCalculator.Setup(new List<MonsterStatCalculator.IntervalGenerator> { new MonsterStatCalculator.IntervalGenerator { min = 1, max = 1000, generator = new Generator { InitialValue = 10000, mode = Generator.Mode.Exponent, r = 1.15 } } });

        
        //Assert.Pass(_trollStageCalculator.GetHP(100).ToString());
        //Assert.Pass(_trollStageCalculator.GetHP(200).ToString());
        //Assert.Pass(_trollStageCalculator.GetHP(800).ToString());
        Assert.Pass(_trollStageCalculator.GetHP(999).ToString());
        Assert.Pass(_trollStageCalculator.GetHP(1000).ToString());

    }
}
