using UnityEngine;

public class BuilderPencil
{
    GameObject startNotation;
    GameObject endNotation;

    public bool IsStart { get => startNotation == null; }

    public void Start(Vector3 pos)
    {
        if (startNotation != null)
            DestroyNotation(startNotation);

        startNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);
    }
    public void End(Vector3 pos)
    {
        if (pos.y != startNotation.transform.position.y)
            return;

        if (endNotation != null)
            DestroyNotation(endNotation);

        endNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);

        ChangeNotationState();

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI == null)
            return;

        homeUI.ShowBuilderPencilBtn();
    }
    public void Cancel()
    {
        if (startNotation != null) DestroyNotation(startNotation);
        if (endNotation != null) DestroyNotation(endNotation);
    }

    private void DestroyNotation(GameObject notation)
    {
        if (notation != null)
            GameObject.Destroy(notation);
    }
    private void ChangeNotationState()
    {
        var posX = endNotation.transform.position.x - startNotation.transform.position.x;
        var posZ = endNotation.transform.position.z - startNotation.transform.position.z;

        var startBarX = CommonFunction.FindChiledByName(startNotation.transform, "X");
        var startBarZ = CommonFunction.FindChiledByName(startNotation.transform, "Z");
        var endBarX = CommonFunction.FindChiledByName(endNotation.transform, "X");
        var endBarZ = CommonFunction.FindChiledByName(endNotation.transform, "Z");

        if (posX >=0 && posZ >= 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
        }
        else if (posX >= 0 && posZ < 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
        }
        else if (posX < 0 && posZ >= 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
        }
        else if (posX < 0 && posZ < 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
        }
    }
    private void ChangePosAndScaleX(GameObject obj, float posX, float scale)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, scale + 1);
        obj.transform.localPosition = new Vector3(posX, obj.transform.localPosition.y, obj.transform.localPosition.z);
    }
    private void ChangePosAndScaleZ(GameObject obj, float posZ, float scale)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, scale + 1);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, posZ);
    }
}