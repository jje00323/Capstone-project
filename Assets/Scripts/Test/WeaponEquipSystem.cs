using UnityEngine;

public class WeaponEquipSystem : MonoBehaviour
{
    [Header("무기 프리팹들")]
    public GameObject[] weaponPrefabs;

    [Header("현재 장착 무기")]
    private GameObject currentWeapon;

    [Header("오른손 본 (장착 위치)")]
    private Transform rightHand;

    /// <summary>
    /// 외부에서 손 본을 설정해주는 함수 (직업 변경 시 호출됨)
    /// </summary>
    public void SetRightHand(Transform newRightHand)
    {
        if (newRightHand == null)
        {
            Debug.LogError("[WeaponEquipSystem] 전달받은 손 본이 null입니다!");
            return;
        }

        rightHand = newRightHand;
        Debug.Log($"[WeaponEquipSystem] 손 본 적용 완료: {rightHand.name}");

        // 무기가 이미 있다면 새 손 본에 다시 붙임
        if (currentWeapon != null)
        {
            currentWeapon.transform.SetParent(rightHand);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 무기를 장착하는 함수
    /// </summary>
    public void EquipWeapon(int index)
    {
        if (weaponPrefabs == null || index < 0 || index >= weaponPrefabs.Length)
        {
            Debug.LogWarning("[WeaponEquipSystem] 무기 인덱스가 잘못되었습니다.");
            return;
        }

        if (rightHand == null)
        {
            Debug.LogError("[WeaponEquipSystem] 손 본이 설정되지 않았습니다!");
            return;
        }

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(weaponPrefabs[index], rightHand);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        Debug.Log($"[WeaponEquipSystem] {weaponPrefabs[index].name} 무기 장착 완료!");
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}