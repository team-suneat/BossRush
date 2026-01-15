using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckOwnerAlive : ConditionTask<Character>
    {
        public string result;

        protected override bool OnCheck()
        {
            if (agent == null)
            {
                result = "Character를 찾을 수 없습니다.";

                return false;
            }

            if (agent.MyVital == null)
            {
                result = "Character의 Vital을 찾을 수 없습니다.";

                return false;
            }

            if (!agent.MyVital.IsAlive)
            {
                result = "이미 사망한 캐릭터입니다.";

                return false;
            }

            result = null;

            return true;
        }

        protected override string info
        {
            get
            {
                return "캐릭터 생존 확인";
            }
        }
    }
}