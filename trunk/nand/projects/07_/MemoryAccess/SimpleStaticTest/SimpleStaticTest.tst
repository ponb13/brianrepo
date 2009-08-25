rom-load SimpleStaticTest.asm,
output-file SimpleStaticTest.out,
compare-to SimpleStaticTest.cmp,
output-list RAM[256]%D1.6.1;

set RAM[0] 256,

repeat 200 {
  ticktock;
}

output;
