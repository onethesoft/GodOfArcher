using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "아이템/업그레이드테이블/펫", fileName = "PetUpgradeTable")]
public class PetUpgradeSystem : UpgradeSystem
{
    public override string GetNextLevelItem(BaseItem consumeItem)
    {
        if (!_tasks.All(x => ((x.TargetItem) is Pet)))
            throw new System.Exception();
        if (!(consumeItem is Pet))
            throw new System.Exception();

        Pet _consumeRune = consumeItem as Pet;
        UpgradeTask _findTask;
        if (_consumeRune.Level < (int)Pet.Rank.S)
            _findTask = _tasks.Find(x => ((x.TargetItem as Pet).Level == (_consumeRune.Level + 1)) && ((x.TargetItem as Pet).type == (_consumeRune.type)));
        else
            _findTask = _tasks.Find(x => ((x.TargetItem as Pet).Level == (_consumeRune.Level + 1)) && ((x.TargetItem as Pet).type == Pet.Type.All));

        if (_findTask == null)
            return null;
        else
            return _findTask.TargetItem.ItemId;
    }
}
