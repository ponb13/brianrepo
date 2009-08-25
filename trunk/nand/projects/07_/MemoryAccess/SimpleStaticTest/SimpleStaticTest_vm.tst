rom-load SimpleStaticTest.vm,
output-file SimpleStaticTest.out,
compare-to SimpleStaticTest.cmp,
output-list RAM[256]%D1.6.1;

set SP 256,

repeat 11 {
  vmstep;
}

output;
