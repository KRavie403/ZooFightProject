using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private float boundary = 20f;

    // 정사각형의 테두리 정보
    public float boundaryXMin = 13.7f;
    public float boundaryXMax = 46.3f;
    public float boundaryZMin = 6.0f;
    public float boundaryZMax = 33.2f;

    private void Update()
    {
        CameraHandler();
        HandleKeyboardInput();
    }

    public void CameraHandler()
    {
        // 마우스 위치 감지
        Vector2 mousePos = Input.mousePosition;

        // 이동 조건 검사 및 이동
        if (mousePos.x < boundary)
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        else if (mousePos.x > Screen.width - boundary)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (mousePos.y < boundary)
            transform.Translate(Vector3.down * moveSpeed * 0.8f * Time.deltaTime);
        else if (mousePos.y > Screen.height - boundary)
            transform.Translate(Vector3.up * moveSpeed * 0.8f * Time.deltaTime);

        // 경계값을 넘어가지 않도록 제한
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, boundaryXMin, boundaryXMax),
            transform.position.y,
            Mathf.Clamp(transform.position.z, boundaryZMin, boundaryZMax)
        );
    }

    private void HandleKeyboardInput()
    {
        // 키보드 입력 감지
        float horizontal = 0;
        float vertical = 0;

        // 화살표 키 입력
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            horizontal += 1;
        if (Input.GetKey(KeyCode.UpArrow))
            vertical += 1;
        if (Input.GetKey(KeyCode.DownArrow))
            vertical -= 1;

        // WASD 키 입력
        //if (Input.GetKey(KeyCode.A))
        //{
        //    horizontal -= 1;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    horizontal += 1;
        //}

        //if (Input.GetKey(KeyCode.W))
        //{
        //    vertical += 1;
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    vertical -= 1;
        //}

        Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 경계값을 넘어가지 않도록 제한
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, boundaryXMin, boundaryXMax),
            transform.position.y,
            Mathf.Clamp(transform.position.z, boundaryZMin, boundaryZMax)
        );
    }
}