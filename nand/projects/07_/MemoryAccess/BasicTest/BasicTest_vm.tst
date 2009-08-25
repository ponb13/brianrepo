rom-load BasicTest.vm,
output-file BasicTest.out,
compare-to BasicTest.cmp,
output-list RAM[256]%D1.6.1 RAM[300]%D1.6.1 RAM[401]%D1.6.1 RAM[402]%D1.6.1 RAM[3006]%D1.6.1 RAM[3012]%D1.6.1 RAM[3015]%D1.6.1 RAM[11]%D1.6.1;

set SP 256,
set Local 300,
set Argument 400,
set This 3000,
set That 3010,

repeat 25 {
  vmstep;
}

output;