# GL Tech 2.1

A GL Tech 2.1 é um motor gráfico 3D (ou 2.5D) baseado em ray-casting criado em Agosto de 2020, mas cujo projeto ainda se encontra sendo lentamente desenvolvido. Projetada para a estrutura do .NET 6 e escrita integralmente em C#, a biblioteca funciona exclusivamente sobre poder de processamento de CPU e sem emprego de implementações gráficas preexistentes tais como OpenGL.

Atualmente, pode ser separada em módulos responsáveis por:

- Síntese gráfica (principal)
- Sistema básico de depuração
- Parenteamento entre entidades
- Comportamento de entidades (scripting)
- Pós processamento
- Tratamento e detecção de colisões
- Compatibilidade com módulos nativos
  - Gerenciamento de memória não gerenciada

Veja um breve vídeo mostrando mapas rendeirzaodos pela engine:
https://youtu.be/_bNiS_YMWdw

## Links de documentação

Você pode acessar a documentação atual da GL Tech 2.1 em https://king-witcher.visualstudio.com/GL-Tech-2.1/_wiki/ (ainda em construção).

Ainda há uma página antiga de documentação em https://king-witcher.github.io/GL-Tech-2.1/. No entanto, a maioria das classes estão desatualizadas ou mudaram de nome nessa documentação e a única que ainda se comporta da mesma forma é a struct Vector, que me lembre. Sei que tenho costumo receber visitas no repositório e, por isso, estou dedicando tempo para do cumentar novamente o funcionamento da biblioteca.

Sinta-se à vontade para interagir na aba "Discussões" ou entrar em contato diretamente comigo pelo e-mail giuseppelanna2000@gmail.com. Don't worry if you don't speak Portuguese.

## Ao visitante

Criei essa biblioteca no início de agosto de 2020, mas sem intenção de fazer dela algo grande ou sequer útil para outras pessoas. Minhas reais motivações por trás dessa engine sempre foram duas: aprender a programar melhor e um desejo quase compulsório por criar algo, independente da utilidade.

Sem sombra de dúvida, tanto meus conhecimentos sobre computação quanto a qualidade do código que escrevo hoje é astronomicamente maior que a que eu escrevia antes de começar a programar essa engine, independentemente da linguagem; portanto, acredito que ela cumpriu muito bem os objetivos iniciais.

Ainda tenho coisa em mente que pode ser adicionada ou refatorada nesse código, mas algumas coisas me desmotivaram a continuar produzindo código diariamente como antes. Entre elas, existem duras limitações de performance que me impedem de adicionar certas funcionalidades sem precisar escolher entre qualidade de código e desempenho. Infelizmente, às vezes lido com esse tipo de coisa, uma vez que o C# não é próprio para o tipo de tarefa que está gambiarramente sendo forçado a fazer.

Leve em conta também que, como ainda estou desenvolvendo essa engine, eu comumente decido renomear ou refatorar completamente o funcionamento de determinados módulos, tornando praticamente inviável utilizar meu código para alguma coisa por enquato. Diante disso, apenas aprecie seu conteúdo como arte, pois é assim que o vejo.

E divirta-se.

Giuseppe Lanna

26-10-2021
