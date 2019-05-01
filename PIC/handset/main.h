#include <18F46K22.h>
#device ADC=10
#device WRITE_EEPROM=ASYNC 

//#FUSES HSH                       //High speed osc with HW enabled 4X PLL
#FUSES PLLEN
#FUSES PRIMARY_ON
#FUSES NOMCLR   
#FUSES WDT4096                   //Watch Dog Timer uses 1:128 Postscale = 4096*4ms =~16.4s
#FUSES PUT                      //Power up timer enabled
#FUSES NOFCMEN                  //Fail-safe clock monitor enabled
#FUSES NOIESO                   //Internal External Switch Over mode enabled
#FUSES NOBROWNOUT               //No brownout reset
#FUSES CCP2C1
#FUSES NOSTVREN                   //Stack full/underflow will not cause reset
#FUSES NOPBADEN                 //PORTB pins are configured as digital I/O on RESET
//#FUSES NOLPT1OSC                //Timer1 configured for higher power operation
#FUSES TIMER3C0              
#FUSES NOLVP                    //No low voltage prgming, B3(PIC16) or B5(PIC18) used for I/O
#FUSES NOXINST                  //Extended set extension and Indexed Addressing mode disabled (Legacy mode)
#FUSES NODEBUG                  //No Debug mode for ICD
#FUSES NOPROTECT                //Code not protected from reading
#FUSES NOCPB                    //No Boot Block code protection
#FUSES NOCPD                    //No EE protection
#FUSES NOWRT                    //Program memory not write protected
#FUSES NOWRTC                   //configuration not registers write protected
#FUSES NOWRTB                   //Boot block not write protected
#FUSES NOWRTD                   //Data EEPROM not write protected
#FUSES NOEBTR                   //Memory not protected from table reads
#FUSES NOEBTRB                  //Boot block not protected from table reads

#USE delay(clock=64000000, crystal=16000000)
#USE rs232(UART1,baud=115200,parity=N,bits=8,stream=controller, ERRORS)
#USE i2c(Master,I2C2,fast=200000,force_hw)

//buttons
//5 outputs
//-----------------------------
#DEFINE KEYPAD1 PIN_B4 //green out
#DEFINE KEYPAD2 PIN_B0 //black out
#DEFINE KEYPAD3 PIN_B1 //blue out
#DEFINE KEYPAD4 PIN_B2 //white out
#DEFINE KEYPAD5 PIN_B3 //red out

//4 inputs with 100k pull-ups
//-----------------------------
#DEFINE KEYPAD6 PIN_D4 //blue in
#DEFINE KEYPAD7 PIN_D5 //green in
#DEFINE KEYPAD8 PIN_D6 //black in
#DEFINE KEYPAD9 PIN_D7 //red in

//---------menu modes
#DEFINE M_BASE 0
#DEFINE M_SPEED 1
#DEFINE M_LIGHT 2
#DEFINE M_FOCUS 3
#DEFINE M_SOLARSYSTEM 4
#DEFINE M_MESSIER 5
#DEFINE M_CALDWELL 6
#DEFINE M_STAR 7
#DEFINE M_NGC 8
#DEFINE M_IC 9
#DEFINE M_TARGET 10
