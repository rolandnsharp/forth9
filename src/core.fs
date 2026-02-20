\ Forth9 core vocabulary
\ Upload to Zeptoforth with: copy and paste into picocom

compile-to-flash

\ === Variables ===
variable count

\ === Counter tools ===
: bump count @ 1 + count ! ;
: show count @ . ;
: reset 0 count ! ;
: step bump show ;

\ === String tools ===
: hello ." hello world" cr ;

\ === Init ===
: init reset ;

compile-to-ram
init
