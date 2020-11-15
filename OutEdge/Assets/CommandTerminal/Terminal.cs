using UnityEngine;
using System.Text;
using System.Collections;
using UnityEngine.Assertions;
using Mirror;

namespace CommandTerminal
{
    public enum TerminalState
    {
        Close,
        OpenSmall,
        OpenFull
    }

    public class Terminal : MonoBehaviour
    {
        [Header("Window")]
        [Range(0, 1)]
        [SerializeField]
        float MaxHeight = 0.7f;

        [SerializeField]
        [Range(0, 1)]
        float SmallTerminalRatio = 0.33f;

        [Range(100, 1000)]
        [SerializeField]
        float ToggleSpeed = 360;

        [SerializeField] string ToggleHotkey      = "`";
        [SerializeField] string ToggleFullHotkey  = "#`";
        [SerializeField] int BufferSize           = 512;

        [Header("input")]
        [SerializeField] Font ConsoleFont;
        [SerializeField] string inputCaret        = ">";
        [SerializeField] bool ShowGUIButtons;
        [SerializeField] bool RightAlignButtons;

        [Header("Theme")]
        [Range(0, 1)]
        [SerializeField] float inputContrast;
        [Range(0, 1)]
        [SerializeField] float inputAlpha         = 0.5f;

        [SerializeField] Color BackgroundColor    = Color.black;
        [SerializeField] Color ForegroundColor    = Color.white;
        [SerializeField] Color ShellColor         = Color.white;
        [SerializeField] Color inputColor         = Color.cyan;
        [SerializeField] Color WarningColor       = Color.yellow;
        [SerializeField] Color ErrorColor         = Color.red;
        [SerializeField] float scrollspeed = 2;

        TerminalState state;
        TextEditor editor_state;
        bool input_fix;
        bool move_cursor;
        bool initial_open; // Used to focus on TextField when console opens
        Rect window;
        float current_open_t;
        float open_target;
        float real_window_size;
        string command_text;
        string cached_command_text;
        Vector2 scroll_position;
        GUIStyle window_style;
        GUIStyle label_style;
        GUIStyle input_style;
        Texture2D background_texture;
        Texture2D input_background_texture;

        public static CommandLog Buffer { get; private set; }
        public static CommandShell Shell { get; private set; }
        public static CommandHistory History { get; private set; }
        public static CommandAutocomplete Autocomplete { get; private set; }
        public static Terminal terminal;

        public static bool IssuedError {
            get { return Shell.IssuedErrorMessage != null; }
        }

        public bool IsClosed {
            get { return state == TerminalState.Close && Mathf.Approximately(current_open_t, open_target); }
        }

        public static void Log(string format, params object[] message) {
            Log(TerminalLogType.ShellMessage, format, message);
        }

        public static void Log(TerminalLogType type, string format, params object[] message) {
            Buffer.HandleLog(string.Format(format, message), type);
            terminal.scroll_position.y = int.MaxValue;
            terminal.alpha = 1;
        }

        public void SetState(TerminalState new_state) {
            input_fix = true;
            cached_command_text = command_text;
            command_text = "";

            switch (new_state) {
                case TerminalState.Close: {
                    open_target = 0;
                    break;
                }
                case TerminalState.OpenSmall: {
                    open_target = Screen.height * MaxHeight * SmallTerminalRatio;
                    if (current_open_t > open_target) {
                        // Prevent resizing from OpenFull to OpenSmall if window y position
                        // is greater than OpenSmall's target
                        open_target = 0;
                        state = TerminalState.Close;
                        return;
                    }
                    real_window_size = open_target;
                    scroll_position.y = int.MaxValue;
                    break;
                }
                case TerminalState.OpenFull:
                default: {
                    real_window_size = Screen.height * MaxHeight;
                    open_target = real_window_size;
                    break;
                }
            }

            state = new_state;
        }

        public void ToggleState(TerminalState new_state) {
            if (state == new_state) {
                SetState(TerminalState.Close);
                alpha = 0f;
            } else {
                SetState(new_state);
            }
        }

        void OnEnable() {
            if (Shell == null)
            {
                Buffer = new CommandLog(BufferSize);
                Shell = new CommandShell();
                History = new CommandHistory();
                Autocomplete = new CommandAutocomplete();
                Shell.RegisterCommands();
            }

            // Hook Unity log events
            //Application.logMessageReceived += HandleUnityLog;
        }

        void OnDisable() {
            //Application.logMessageReceived -= HandleUnityLog;
        }

        void Start() {
            if (Application.isPlaying && !transform.parent.parent.GetComponent<NetworkIdentity>().isLocalPlayer) { return; }
            terminal = this;
            if (ConsoleFont == null) {
                ConsoleFont = Font.CreateDynamicFontFromOSFont("Courier New", 16);
                Debug.LogWarning("Command Console Warning: Please assign a font.");
            }

            command_text = "";
            cached_command_text = command_text;
            Assert.AreNotEqual(ToggleHotkey.ToLower(), "return", "Return is not a valid ToggleHotkey");

            SetupWindow();
            Setupinput();
            SetupLabels();

            if (IssuedError) {
                Log(TerminalLogType.Error, "Error: {0}", Shell.IssuedErrorMessage);
            }

            foreach (var command in Shell.Commands) {
                Autocomplete.Register(command.Key);
            }
        }

        float alpha = 0;
        public float fade_rate = 0.005f;

        void OnGUI()
        {
            if (!UIManager.ui.interacting)
            {
                if(state == TerminalState.Close)
                {
                    if (InputManager.GetKeyDown("Chat"))
                    {
                        SetState(TerminalState.OpenSmall);
                        initial_open = true;
                    }
                    else if (InputManager.GetKeyDown("Command"))
                    {
                        SetState(TerminalState.OpenSmall);
                        initial_open = true;
                        command_text = "/";
                        cached_command_text = command_text;
                    }
                    else if (Event.current.Equals(Event.KeyboardEvent(ToggleFullHotkey)))
                    {
                        SetState(TerminalState.OpenFull);
                        initial_open = true;
                    }
                    if(alpha > 0)
                    {
                        Color c = BackgroundColor;
                        c.a = 0.25f;
                        background_texture = new Texture2D(1, 1);
                        background_texture.SetPixel(0, 0, c);
                        background_texture.Apply();

                        GUIStyle style = new GUIStyle();
                        style.normal.background = background_texture;
                        style.padding = new RectOffset(4, 4, 4, 4);
                        c = ForegroundColor;
                        c.a = alpha;
                        style.normal.textColor = c;
                        style.font = ConsoleFont;
                        window = GUILayout.Window(88, new Rect(0,0,Screen.width, Screen.height * MaxHeight * SmallTerminalRatio), DrawPreview, "", style);
                        alpha -= fade_rate;
                    }
                }

                scroll_position -= Input.mouseScrollDelta * scrollspeed;

                if (ShowGUIButtons)
                {
                    DrawGUIButtons();
                }

                if (IsClosed)
                {
                    return;
                }

                HandleOpenness();
                window = GUILayout.Window(88, window, DrawConsole, "", window_style);
            }
        }

        void SetupWindow() {
            real_window_size = Screen.height * MaxHeight / 3;
            window = new Rect(0, current_open_t - real_window_size, Screen.width, real_window_size);

            // Set background color
            background_texture = new Texture2D(1, 1);
            background_texture.SetPixel(0, 0, BackgroundColor);
            background_texture.Apply();

            window_style = new GUIStyle();
            window_style.normal.background = background_texture;
            window_style.padding = new RectOffset(4, 4, 4, 4);
            window_style.normal.textColor = ForegroundColor;
            window_style.font = ConsoleFont;
        }

        void SetupLabels() {
            label_style = new GUIStyle();
            label_style.font = ConsoleFont;
            label_style.normal.textColor = ForegroundColor;
            label_style.wordWrap = true;
        }

        void Setupinput() {
            input_style = new GUIStyle();
            input_style.padding = new RectOffset(4, 4, 4, 4);
            input_style.font = ConsoleFont;
            input_style.fixedHeight = ConsoleFont.fontSize * 1.6f;
            input_style.normal.textColor = inputColor;

            var dark_background = new Color();
            dark_background.r = BackgroundColor.r - inputContrast;
            dark_background.g = BackgroundColor.g - inputContrast;
            dark_background.b = BackgroundColor.b - inputContrast;
            dark_background.a = inputAlpha;

            input_background_texture = new Texture2D(1, 1);
            input_background_texture.SetPixel(0, 0, dark_background);
            input_background_texture.Apply();
            input_style.normal.background = input_background_texture;
        }

        void DrawPreview(int Window2D)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0,int.MaxValue), false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.FlexibleSpace();
            DrawLogs(alpha);
            GUILayout.EndScrollView();
        }

        void DrawLogs(float alpha)
        {
            foreach (var log in Buffer.Logs)
            {
                Color color = GetLogColor(log.type);
                color.a = alpha;
                GUIStyle style = new GUIStyle();
                style.font = Font.CreateDynamicFontFromOSFont(ConsoleFont.fontNames, 20);
                style.normal.textColor = ForegroundColor;
                style.wordWrap = true;
                GUILayout.Label(log.message, style);
            }
        }

        void DrawConsole(int Window2D) {
            GUILayout.BeginVertical();

            scroll_position = GUILayout.BeginScrollView(scroll_position, false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.FlexibleSpace();
            DrawLogs();
            GUILayout.EndScrollView();

            if (move_cursor) {
                CursorToEnd();
                move_cursor = false;
            }
            if (Event.current.Equals(Event.KeyboardEvent("escape")))
            {
                SetState(TerminalState.Close);
                alpha = 0;
            }
            else if (Event.current.Equals(Event.KeyboardEvent("return"))
              || Event.current.Equals(Event.KeyboardEvent("[enter]")))
            {
                EnterCommand();
            }
            else if (Event.current.Equals(Event.KeyboardEvent("up")))
            {
                command_text = History.Previous();
                move_cursor = true;
            }
            else if (Event.current.Equals(Event.KeyboardEvent("down")))
            {
                command_text = History.Next();
            }

            else if (Event.current.Equals(Event.KeyboardEvent("tab")))
            {
                CompleteCommand();
                move_cursor = true; // Wait till next draw call
            }

            GUILayout.BeginHorizontal();

            if (inputCaret != "") {
                GUILayout.Label(inputCaret, input_style, GUILayout.Width(ConsoleFont.fontSize));
            }

            GUI.SetNextControlName("command_text_field");
            command_text = GUILayout.TextField(command_text, input_style);

            if (input_fix && command_text.Length > 0) {
                command_text = cached_command_text; // Otherwise the TextField picks up the ToggleHotkey character event
                input_fix = false;                  // Prevents checking string Length every draw call
            }

            if (initial_open) {
                GUI.FocusControl("command_text_field");
                //initial_open = false;
            }

            if (ShowGUIButtons && GUILayout.Button("| run", input_style, GUILayout.Width(Screen.width / 10))) {
                EnterCommand();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void DrawLogs() {
            foreach (var log in Buffer.Logs) {
                label_style.normal.textColor = GetLogColor(log.type);
                GUILayout.Label(log.message, label_style);
            }
        }

        void DrawGUIButtons() {
            int size = ConsoleFont.fontSize;
            float x_position = RightAlignButtons ? Screen.width - 7 * size : 0;

            // 7 is the number of chars in the button plus some padding, 2 is the line height.
            // The layout will resize according to the font size.
            GUILayout.BeginArea(new Rect(x_position, current_open_t, 7 * size, size * 2));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Small", window_style)) {
                ToggleState(TerminalState.OpenSmall);
            } else if (GUILayout.Button("Full", window_style)) {
                ToggleState(TerminalState.OpenFull);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void HandleOpenness() {
            float dt = ToggleSpeed * Time.unscaledDeltaTime;

            if (current_open_t < open_target) {
                current_open_t += dt;
                if (current_open_t > open_target) current_open_t = open_target;
            } else if (current_open_t > open_target) {
                current_open_t -= dt;
                if (current_open_t < open_target) current_open_t = open_target;
            } else {
                if (input_fix) {
                    input_fix = false;
                }
                return; // Already at target
            }

            window = new Rect(0, current_open_t - real_window_size, Screen.width, real_window_size);
        }

        void EnterCommand()
        {
            if (command_text[0] == '/')
            {
                command_text = command_text.Remove(0, 1);
                Log(TerminalLogType.input, "{0}", command_text);
                Shell.RunCommand(command_text);
                History.Push(command_text);

                if (IssuedError)
                {
                    Log(TerminalLogType.Error, "Error: {0}", Shell.IssuedErrorMessage);
                }
            }
            else
            {
                Communicator.comm.CmdSendMsg("<" + (Communicator.playerName.Trim() == "" ? "Blument" : Communicator.playerName.Trim()) + "> : " + command_text);
            }

            command_text = "";
            scroll_position.y = int.MaxValue;
        }

        void CompleteCommand() {
            string head_text = command_text;
            int format_width = 0;

            string[] completion_buffer = Autocomplete.Complete(ref head_text, ref format_width);
            int completion_length = completion_buffer.Length;

            if (completion_length != 0) {
                command_text = head_text;
            }

            if (completion_length > 1) {
                // Print possible completions
                var log_buffer = new StringBuilder();

                foreach (string completion in completion_buffer) {
                    log_buffer.Append(completion.PadRight(format_width + 4));
                }

                Log("{0}", log_buffer);
                scroll_position.y = int.MaxValue;
            }
        }

        void CursorToEnd() {
            if (editor_state == null) {
                editor_state = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            }

            editor_state.MoveCursorToPosition(new Vector2(999, 999));
        }

        Color GetLogColor(TerminalLogType type) {
            switch (type) {
                case TerminalLogType.Message: return ForegroundColor;
                case TerminalLogType.Warning: return WarningColor;
                case TerminalLogType.input: return inputColor;
                case TerminalLogType.ShellMessage: return ShellColor;
                default: return ErrorColor;
            }
        }
    }
}