# GL Tech 2.1

A GL Tech 2.1 é um motor gráfico 3D (ou 2.5D) baseado em ray-casting criado em Agosto de 2020, mas cujo projeto ainda se encontra sendo lentamente desenvolvido. Projetada para a estrutura do .NET 5 e escrita integralmente em C#, a biblioteca funciona exclusivamente sobre poder de processamento de CPU e sem emprego de implementações gráficas preexistentes tais como OpenGL.

Atualmente, pode ser decomposta princiaplmente em sub sistemas responsáveis por:

- Depuração
- Síntese gráfica
- Posicionamento de entidades
- Comportamento de entidades
- Gerenciamento de cenas
- Tratamento e detecção de colisões
- Suporte a código nativo e memória não gerenciada

## Mensagem ao leitor

Criei essa biblioteca no início de agosto de 2020, mas sem intenção de fazer dela algo grande ou sequer útil para outras pessoas. Minhas reais motivações por trás dessa engine sempre foram duas: aprender a programar melhor e um desejo quase compulsório por criar algo, independente da utilidade.

Sem sombra de dúvida, tanto meus conhecimentos sobre computação quanto a qualidade do código que escrevo hoje é astronomicamente maior que a que eu escrevia antes de começar a programar essa engine, independentemente da linguagem; portanto, acredito que ela cumpriu muito bem os objetivos iniciais.

Ainda tenho coisa em mente que pode ser adicionada ou refatorada nesse código, mas algumas coisas me desmotivaram a continuar produzindo código diariamente como antes. Entre elas, existem duras limitações de performance que me impedem de adicionar certas funcionalidades sem precisar escolher entre qualidade de código e desempenho. Infelizmente, às vezes lido com esse tipo de coisa, uma vez que o C# não é próprio para o tipo de tarefa que está gambiarramente sendo forçado a fazer.

Leve em conta também que, como ainda estou desenvolvendo essa engine, eu comumente decido renomear ou refatorar completamente o funcionamento de determinados módulos, tornando praticamente inviável utilizar meu código para alguma coisa por enquato. Diante disso, apenas aprecie seu conteúdo como arte, pois é assim que o vejo.

E divirta-se.

Giuseppe Lanna
26-10-2021
