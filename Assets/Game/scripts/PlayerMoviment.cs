using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Define a velocidade de movimento do personagem. (É pública, então pode ser ajustada pelo Inspector).
    public float moveSpeed = 5f;
    // Guarda a entrada do jogador no formato Vector2 (x para horizontal e y para vertical).
    private Vector2 moveInput;
    // Referência ao CharacterController, um componente da Unity que facilita movimentação sem precisar usar Rigidbody.
    private CharacterController controller;
    // Declara uma variável controls do tipo InputActions, que é uma classe gerada automaticamente pelo Input System quando criamos um Input Action no Unity.
    private InputActions inputActions;

    private Transform myCamera;

    // Awake() é chamado antes do Start()
    private void Awake()
    {
        // Obtém o CharacterController componente do jogador.
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Esconder e travar o cursor no centro da tela
        // Obtém a referência à câmara principal.
        myCamera = Camera.main.transform;
    }

    // Update() é chamado a cada frame.
    private void Update()
    {
        // Cria um Vector3 usando o moveInput (x para esquerda/direita e y para frente/trás).
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // Define a direção com base na direção e que a camera aponta
        moveDirection = myCamera.TransformDirection(moveDirection);
        moveDirection.y = 0;

        // Aplica a direção do movimento ao CharacterController.
        // Multiplica por Time.deltaTime para garantir que o movimento seja suavizado independentemente do FPS.
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        // Elimina o movimento vertical para evitar que o personagem seja deslocado verticalmente.
        controller.Move(new Vector3(0, -9.81f, 0) * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            // Quaternion.Slerp suaviza a rotação de um objeto em direção a uma nova direção desejada recebendo como parametro a rotação atual, rotação desejada e controle de velocidade da interpolação.
            // Quaternion.LookRotation(moveDirection) cria uma rotação que faz o objeto olhar para a direção moveDirection
            // Time.deltaTime * 10 é usado para suavizar a rotação, evitando que a rotação seja instantânea.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * 10);
        }

    }

    // OnEnable() é chamado automaticamente quando o script é ativado.
    private void OnEnable()
    {
        // Instancia o sistema de entrada baseado no arquivo de Input Actions criado na Unity.
        inputActions = new InputActions();
        // Habilita o Input System para capturar comandos do jogador.
        inputActions.Enable();
        // Sempre que o jogador pressiona ou solta as teclas WASD ou o analógico do controle, o evento Move.performed ou Move.canceled é chamado.
        // O método OnMove será executado para processar esses inputs.
        inputActions.Game.Move.performed += OnMove;
        inputActions.Game.Move.canceled += OnMove;
    }

    // OnDisable() é chamado automaticamente quando o script é desativado.
    private void OnDisable()
    {
        // Isso evita memory leaks e garante que o método OnMove não seja chamado após a desativação do GameObject.
        inputActions.Game.Move.performed -= OnMove;
        inputActions.Game.Move.canceled -= OnMove;
        // Isso impede que o jogador continue enviando comandos quando o GameObject estiver desativado.
        inputActions.Disable();
    }

    // OnMove é chamado automaticamente pelo Input System sempre que o jogador move o controle ou aperta WASD.
    public void OnMove(InputAction.CallbackContext context)
    {
        // context.ReadValue<Vector2>() → Lê o valor do input e armazena em moveInput.
        moveInput = context.ReadValue<Vector2>();
    }
}
