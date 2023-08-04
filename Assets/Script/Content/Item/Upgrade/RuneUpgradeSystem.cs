using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "아이템/업그레이드테이블/룬", fileName = "RuneUpgradeTable")]
public class RuneUpgradeSystem : UpgradeSystem
{
    

    public override string GetNextLevelItem(BaseItem consumeItem)
    {
        if(!_tasks.All(x => ((x.TargetItem) is Rune) ))
            throw new System.Exception();
        if(!(consumeItem is Rune))
            throw new System.Exception();

        Rune _consumeRune = consumeItem as Rune;
        UpgradeTask _findTask;
        if (_consumeRune.Level < (int)Rune.Rank.S)
            _findTask = _tasks.Find(x => ((x.TargetItem as Rune).Level == (_consumeRune.Level + 1)) && ((x.TargetItem as Rune).type == (_consumeRune.type)));
        else
            _findTask = _tasks.Find(x => ((x.TargetItem as Rune).Level == (_consumeRune.Level + 1)) && ((x.TargetItem as Rune).type == Rune.Type.All));

        if (_findTask == null)
            return null;
        else
            return _findTask.TargetItem.ItemId;


    }
}
