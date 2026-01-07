using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Pattern")]
    public class ActionStartPattern : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.patternSystem != null)
            // {
            //     agent.patternSystem.StartPattern();
            // }
            // else
            // {
            //     Log.Error("Failed to action start pattern. agent.patternSystem is null. path: {0}", agent.GetHierarchyPath());
            // }
        }
    }
}