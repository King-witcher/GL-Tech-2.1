using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    /// <summary>
    ///     Represents a keyboard key.
    /// </summary>
    public enum Key
    {
        //
        // Resumo:
        //     O bitmask para extrair os modificadores de um valor de tecla.
        Modifiers = -65536,
        //
        // Resumo:
        //     Nenhuma tecla pressionada.
        None = 0,
        //
        // Resumo:
        //     O botão esquerdo do mouse.
        LButton = 1,
        //
        // Resumo:
        //     O botão direito do mouse.
        RButton = 2,
        //
        // Resumo:
        //     A tecla CANCEL.
        Cancel = 3,
        //
        // Resumo:
        //     O botão do meio do mouse (mouse de três botões).
        MButton = 4,
        //
        // Resumo:
        //     O primeiro botão x do mouse (mouse de cinco botões).
        XButton1 = 5,
        //
        // Resumo:
        //     O segundo botão x do mouse (mouse de cinco botões).
        XButton2 = 6,
        //
        // Resumo:
        //     A tecla BACKSPACE.
        Back = 8,
        //
        // Resumo:
        //     A tecla TAB.
        Tab = 9,
        //
        // Resumo:
        //     A tecla LINEFEED.
        LineFeed = 10,
        //
        // Resumo:
        //     A tecla CLEAR.
        Clear = 12,
        //
        // Resumo:
        //     A tecla RETURN.
        Return = 13,
        //
        // Resumo:
        //     A tecla ENTER.
        Enter = 13,
        //
        // Resumo:
        //     A tecla SHIFT.
        ShiftKey = 16,
        //
        // Resumo:
        //     A tecla CTRL.
        ControlKey = 17,
        //
        // Resumo:
        //     A tecla ALT.
        Menu = 18,
        //
        // Resumo:
        //     A tecla PAUSE.
        Pause = 19,
        //
        // Resumo:
        //     A tecla CAPS LOCK.
        Capital = 20,
        //
        // Resumo:
        //     A tecla CAPS LOCK.
        CapsLock = 20,
        //
        // Resumo:
        //     A tecla do modo IME Kana.
        KanaMode = 21,
        //
        // Resumo:
        //     A tecla do modo IME Hanguel. (mantida para compatibilidade, use HangulMode)
        HanguelMode = 21,
        //
        // Resumo:
        //     A tecla do modo IME Hangul.
        HangulMode = 21,
        //
        // Resumo:
        //     A tecla do modo IME Junja.
        JunjaMode = 23,
        //
        // Resumo:
        //     A tecla do modo final do IME.
        FinalMode = 24,
        //
        // Resumo:
        //     A tecla do modo IME Hanja.
        HanjaMode = 25,
        //
        // Resumo:
        //     A tecla do modo IME Kanji.
        KanjiMode = 25,
        //
        // Resumo:
        //     A tecla ESC.
        Escape = 27,
        //
        // Resumo:
        //     A tecla de conversão do IME.
        IMEConvert = 28,
        //
        // Resumo:
        //     A tecla IME nonconvert.
        IMENonconvert = 29,
        //
        // Resumo:
        //     A tecla de aceitação do IME, substitui System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Resumo:
        //     A tecla de aceitação do IME. Obsoleta, use System.Windows.Forms.Keys.IMEAccept
        //     em seu lugar.
        IMEAceept = 30,
        //
        // Resumo:
        //     A tecla de alteração do modo IME.
        IMEModeChange = 31,
        //
        // Resumo:
        //     A tecla BARRA DE ESPAÇOS.
        Space = 32,
        //
        // Resumo:
        //     A tecla PAGE UP.
        Prior = 33,
        //
        // Resumo:
        //     A tecla PAGE UP.
        PageUp = 33,
        //
        // Resumo:
        //     A tecla PAGE DOWN.
        Next = 34,
        //
        // Resumo:
        //     A tecla PAGE DOWN.
        PageDown = 34,
        //
        // Resumo:
        //     A tecla END.
        End = 35,
        //
        // Resumo:
        //     A tecla HOME.
        Home = 36,
        //
        // Resumo:
        //     A tecla SETA PARA A ESQUERDA.
        Left = 37,
        //
        // Resumo:
        //     A tecla SETA PARA CIMA.
        Up = 38,
        //
        // Resumo:
        //     A tecla SETA PARA A DIREITA.
        Right = 39,
        //
        // Resumo:
        //     A tecla SETA PARA BAIXO.
        Down = 40,
        //
        // Resumo:
        //     A tecla SELECT.
        Select = 41,
        //
        // Resumo:
        //     A tecla PRINT.
        Print = 42,
        //
        // Resumo:
        //     A tecla EXECUTE.
        Execute = 43,
        //
        // Resumo:
        //     A tecla PRINT SCREEN.
        Snapshot = 44,
        //
        // Resumo:
        //     A tecla PRINT SCREEN.
        PrintScreen = 44,
        //
        // Resumo:
        //     A tecla INS.
        Insert = 45,
        //
        // Resumo:
        //     A tecla DEL.
        Delete = 46,
        //
        // Resumo:
        //     A tecla HELP.
        Help = 47,
        //
        // Resumo:
        //     A tecla 0.
        D0 = 48,
        //
        // Resumo:
        //     A tecla 1.
        D1 = 49,
        //
        // Resumo:
        //     A tecla 2.
        D2 = 50,
        //
        // Resumo:
        //     A tecla 3.
        D3 = 51,
        //
        // Resumo:
        //     A tecla 4.
        D4 = 52,
        //
        // Resumo:
        //     A tecla 5.
        D5 = 53,
        //
        // Resumo:
        //     A tecla 6.
        D6 = 54,
        //
        // Resumo:
        //     A tecla 7.
        D7 = 55,
        //
        // Resumo:
        //     A tecla 8.
        D8 = 56,
        //
        // Resumo:
        //     A tecla 9.
        D9 = 57,
        //
        // Resumo:
        //     A tecla A.
        A = 65,
        //
        // Resumo:
        //     A tecla B.
        B = 66,
        //
        // Resumo:
        //     A tecla C.
        C = 67,
        //
        // Resumo:
        //     A tecla D.
        D = 68,
        //
        // Resumo:
        //     A tecla E.
        E = 69,
        //
        // Resumo:
        //     A tecla F.
        F = 70,
        //
        // Resumo:
        //     A tecla G.
        G = 71,
        //
        // Resumo:
        //     A tecla H.
        H = 72,
        //
        // Resumo:
        //     A tecla I.
        I = 73,
        //
        // Resumo:
        //     A tecla J.
        J = 74,
        //
        // Resumo:
        //     A tecla K.
        K = 75,
        //
        // Resumo:
        //     A tecla L.
        L = 76,
        //
        // Resumo:
        //     A tecla M.
        M = 77,
        //
        // Resumo:
        //     A tecla N.
        N = 78,
        //
        // Resumo:
        //     A tecla O.
        O = 79,
        //
        // Resumo:
        //     A tecla P.
        P = 80,
        //
        // Resumo:
        //     A tecla Q.
        Q = 81,
        //
        // Resumo:
        //     A tecla R.
        R = 82,
        //
        // Resumo:
        //     A tecla S.
        S = 83,
        //
        // Resumo:
        //     A tecla T.
        T = 84,
        //
        // Resumo:
        //     A tecla U.
        U = 85,
        //
        // Resumo:
        //     A tecla V.
        V = 86,
        //
        // Resumo:
        //     A tecla W.
        W = 87,
        //
        // Resumo:
        //     A tecla X.
        X = 88,
        //
        // Resumo:
        //     A tecla Y.
        Y = 89,
        //
        // Resumo:
        //     A tecla Z.
        Z = 90,
        //
        // Resumo:
        //     A tecla esquerda do logotipo do Windows (Microsoft Natural Keyboard).
        LWin = 91,
        //
        // Resumo:
        //     A tecla direita do logotipo do Windows (Microsoft Natural Keyboard).
        RWin = 92,
        //
        // Resumo:
        //     A tecla Aplicativo (Microsoft Natural Keyboard).
        Apps = 93,
        //
        // Resumo:
        //     A tecla de suspensão do computador.
        Sleep = 95,
        //
        // Resumo:
        //     A tecla 0 no teclado numérico.
        NumPad0 = 96,
        //
        // Resumo:
        //     A tecla 1 no teclado numérico.
        NumPad1 = 97,
        //
        // Resumo:
        //     A tecla 2 no teclado numérico.
        NumPad2 = 98,
        //
        // Resumo:
        //     A tecla 3 no teclado numérico.
        NumPad3 = 99,
        //
        // Resumo:
        //     A tecla 4 no teclado numérico.
        NumPad4 = 100,
        //
        // Resumo:
        //     A tecla 5 no teclado numérico.
        NumPad5 = 101,
        //
        // Resumo:
        //     A tecla 6 no teclado numérico.
        NumPad6 = 102,
        //
        // Resumo:
        //     A tecla 7 no teclado numérico.
        NumPad7 = 103,
        //
        // Resumo:
        //     A tecla 8 no teclado numérico.
        NumPad8 = 104,
        //
        // Resumo:
        //     A tecla 9 no teclado numérico.
        NumPad9 = 105,
        //
        // Resumo:
        //     A tecla Multiply.
        Multiply = 106,
        //
        // Resumo:
        //     A tecla Adicionar.
        Add = 107,
        //
        // Resumo:
        //     A tecla Separador.
        Separator = 108,
        //
        // Resumo:
        //     A tecla Subtrair.
        Subtract = 109,
        //
        // Resumo:
        //     A tecla Decimal.
        Decimal = 110,
        //
        // Resumo:
        //     A tecla Dividir.
        Divide = 111,
        //
        // Resumo:
        //     A tecla F1.
        F1 = 112,
        //
        // Resumo:
        //     A tecla F2.
        F2 = 113,
        //
        // Resumo:
        //     A tecla F3.
        F3 = 114,
        //
        // Resumo:
        //     A tecla F4.
        F4 = 115,
        //
        // Resumo:
        //     A tecla F5.
        F5 = 116,
        //
        // Resumo:
        //     A tecla F6.
        F6 = 117,
        //
        // Resumo:
        //     A tecla F7.
        F7 = 118,
        //
        // Resumo:
        //     A tecla F8.
        F8 = 119,
        //
        // Resumo:
        //     A tecla F9.
        F9 = 120,
        //
        // Resumo:
        //     A tecla F10.
        F10 = 121,
        //
        // Resumo:
        //     A tecla F11.
        F11 = 122,
        //
        // Resumo:
        //     A tecla F12.
        F12 = 123,
        //
        // Resumo:
        //     A tecla F13.
        F13 = 124,
        //
        // Resumo:
        //     A tecla F14.
        F14 = 125,
        //
        // Resumo:
        //     A tecla F15.
        F15 = 126,
        //
        // Resumo:
        //     A tecla F16.
        F16 = 127,
        //
        // Resumo:
        //     A tecla F17.
        F17 = 128,
        //
        // Resumo:
        //     A tecla F18.
        F18 = 129,
        //
        // Resumo:
        //     A tecla F19.
        F19 = 130,
        //
        // Resumo:
        //     A tecla F20.
        F20 = 131,
        //
        // Resumo:
        //     A tecla F21.
        F21 = 132,
        //
        // Resumo:
        //     A tecla F22.
        F22 = 133,
        //
        // Resumo:
        //     A tecla F23.
        F23 = 134,
        //
        // Resumo:
        //     A tecla F24.
        F24 = 135,
        //
        // Resumo:
        //     A tecla NUM LOCK.
        NumLock = 144,
        //
        // Resumo:
        //     A tecla SCROLL LOCK.
        Scroll = 145,
        //
        // Resumo:
        //     A tecla SHIFT esquerda.
        LShiftKey = 160,
        //
        // Resumo:
        //     A tecla SHIFT direita.
        RShiftKey = 161,
        //
        // Resumo:
        //     A tecla CTRL esquerda.
        LControlKey = 162,
        //
        // Resumo:
        //     A tecla CTRL direita.
        RControlKey = 163,
        //
        // Resumo:
        //     A tecla ALT esquerda.
        LMenu = 164,
        //
        // Resumo:
        //     A tecla ALT direita.
        RMenu = 165,
        //
        // Resumo:
        //     A tecla Voltar do navegador (Windows 2000 ou posterior).
        BrowserBack = 166,
        //
        // Resumo:
        //     A tecla Avançar do navegador (Windows 2000 ou posterior).
        BrowserForward = 167,
        //
        // Resumo:
        //     A tecla Atualizar do navegador (Windows 2000 ou posterior).
        BrowserRefresh = 168,
        //
        // Resumo:
        //     A tecla Parar do navegador (Windows 2000 ou posterior).
        BrowserStop = 169,
        //
        // Resumo:
        //     A tecla Pesquisar do navegador (Windows 2000 ou posterior).
        BrowserSearch = 170,
        //
        // Resumo:
        //     A tecla Favoritos do navegador (Windows 2000 ou posterior).
        BrowserFavorites = 171,
        //
        // Resumo:
        //     A tecla Início do navegador (Windows 2000 ou posterior).
        BrowserHome = 172,
        //
        // Resumo:
        //     A tecla Ativar mudo (Windows 2000 ou posterior).
        VolumeMute = 173,
        //
        // Resumo:
        //     A tecla Abaixar Volume (Windows 2000 ou posterior).
        VolumeDown = 174,
        //
        // Resumo:
        //     A tecla Aumentar Volume (Windows 2000 ou posterior).
        VolumeUp = 175,
        //
        // Resumo:
        //     A tecla Próxima Faixa de Mídia (Windows 2000 ou posterior).
        MediaNextTrack = 176,
        //
        // Resumo:
        //     A tecla Faixa Anterior de Mídia (Windows 2000 ou posterior).
        MediaPreviousTrack = 177,
        //
        // Resumo:
        //     A tecla Parar Mídia (Windows 2000 ou posterior).
        MediaStop = 178,
        //
        // Resumo:
        //     A tecla Reproduzir/Pausar Mídia (Windows 2000 ou posterior).
        MediaPlayPause = 179,
        //
        // Resumo:
        //     A tecla Iniciar Email (Windows 2000 ou posterior).
        LaunchMail = 180,
        //
        // Resumo:
        //     A tecla Selecionar Mídia (Windows 2000 ou posterior).
        SelectMedia = 181,
        //
        // Resumo:
        //     A tecla Iniciar Aplicativo Um (Windows 2000 ou posterior).
        LaunchApplication1 = 182,
        //
        // Resumo:
        //     A tecla Iniciar Aplicativo Dois (Windows 2000 ou posterior).
        LaunchApplication2 = 183,
        //
        // Resumo:
        //     A tecla de ponto e vírgula do OEM em um teclado padrão dos EUA (Windows 2000
        //     ou posterior).
        OemSemicolon = 186,
        //
        // Resumo:
        //     A tecla 1 do OEM.
        Oem1 = 186,
        //
        // Resumo:
        //     A tecla de mais do OEM no teclado de qualquer país/região (Windows 2000 ou posterior).
        Oemplus = 187,
        //
        // Resumo:
        //     A tecla de vírgula do OEM no teclado de qualquer país/região (Windows 2000 ou
        //     posterior).
        Oemcomma = 188,
        //
        // Resumo:
        //     A tecla de menos do OEM no teclado de qualquer país/região (Windows 2000 ou posterior).
        OemMinus = 189,
        //
        // Resumo:
        //     A tecla de ponto do OEM no teclado de qualquer país/região (Windows 2000 ou posterior).
        OemPeriod = 190,
        //
        // Resumo:
        //     A tecla de ponto de interrogação do OEM em um teclado padrão dos EUA (Windows
        //     2000 ou posterior).
        OemQuestion = 191,
        //
        // Resumo:
        //     A tecla 2 do OEM.
        Oem2 = 191,
        //
        // Resumo:
        //     A tecla de til do OEM em um teclado padrão dos EUA (Windows 2000 ou posterior).
        Oemtilde = 192,
        //
        // Resumo:
        //     A tecla 3 do OEM.
        Oem3 = 192,
        //
        // Resumo:
        //     A tecla de parêntese de abertura do OEM em um teclado padrão dos EUA (Windows
        //     2000 ou posterior).
        OemOpenBrackets = 219,
        //
        // Resumo:
        //     A tecla 4 do OEM.
        Oem4 = 219,
        //
        // Resumo:
        //     A tecla de barra vertical do OEM em um teclado padrão dos EUA (Windows 2000 ou
        //     posterior).
        OemPipe = 220,
        //
        // Resumo:
        //     A tecla 5 do OEM.
        Oem5 = 220,
        //
        // Resumo:
        //     A tecla de parêntese de fechamento do OEM em um teclado padrão dos EUA (Windows
        //     2000 ou posterior).
        OemCloseBrackets = 221,
        //
        // Resumo:
        //     A tecla 6 do OEM.
        Oem6 = 221,
        //
        // Resumo:
        //     A tecla de aspas simples/duplas do OEM em um teclado padrão dos EUA (Windows
        //     2000 ou posterior).
        OemQuotes = 222,
        //
        // Resumo:
        //     A tecla 7 do OEM.
        Oem7 = 222,
        //
        // Resumo:
        //     A tecla 8 do OEM.
        Oem8 = 223,
        //
        // Resumo:
        //     A tecla de colchete angular ou barra invertida do OEM no teclado de 102 teclas
        //     RT (Windows 2000 ou posterior).
        OemBackslash = 226,
        //
        // Resumo:
        //     A tecla 102 do OEM.
        Oem102 = 226,
        //
        // Resumo:
        //     A tecla PROCESS KEY.
        ProcessKey = 229,
        //
        // Resumo:
        //     Usada para passar a caracteres Unicode como se fossem pressionamentos de teclas.
        //     O valor da tecla Packet é a palavra inferior de um valor de tecla virtual de
        //     32 bits usado para métodos de entrada diferentes do teclado.
        Packet = 231,
        //
        // Resumo:
        //     A tecla ATTN.
        Attn = 246,
        //
        // Resumo:
        //     A tecla CRSEL.
        Crsel = 247,
        //
        // Resumo:
        //     A tecla EXSEL.
        Exsel = 248,
        //
        // Resumo:
        //     A tecla ERASE EOF.
        EraseEof = 249,
        //
        // Resumo:
        //     A tecla PLAY.
        Play = 250,
        //
        // Resumo:
        //     A tecla ZOOM.
        Zoom = 251,
        //
        // Resumo:
        //     Uma constante reservada para uso futuro.
        NoName = 252,
        //
        // Resumo:
        //     A tecla PA1.
        Pa1 = 253,
        //
        // Resumo:
        //     A tecla CLEAR.
        OemClear = 254,
        //
        // Resumo:
        //     O bitmask para extrair um código de tecla de um valor de tecla.
        KeyCode = 65535,
        //
        // Resumo:
        //     A tecla modificadora SHIFT.
        Shift = 65536,
        //
        // Resumo:
        //     A tecla modificadora CTRL.
        Control = 131072,
        //
        // Resumo:
        //     A tecla modificadora ALT.
        Alt = 262144
    }
}
