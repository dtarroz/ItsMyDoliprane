export class TimeProgressBar extends HTMLElement {

    constructor() {
        super();
        this.innerHTML = `<div tabindex="0">
                            <div class="time-title-container">
                              ${this.getCaptionHtml()}
                              ${this.getMedicationHtml()}
                            </div>
                            ${this.getTooltipHtml()}
                            ${this.getProgressBarHtml()}
                          </div>`;
    }

    isProgressEnd() {
        return this.current() >= this.max();
    }

    getMedicationHtml() {
        let numberMedication = this.numberMedication();
        let html = '';
        for (let i = 0; i < numberMedication; i++) {
            html += '<svg class="time-title-medication" viewBox="0 0 58 58"><use xlink:href="#doliprane"></use></svg>';
        }
        return html;
    }

    getTooltipHtml() {
        return `<span class="tooltip">${this.tooltip()}</span>`;
    }

    getCaptionHtml() {
        let html = `<span class="time-title">${this.caption()}`;
        if (this.isProgressEnd())
            html += `<svg class="time-title-end" viewBox="0 0 512 512">
<path d="M0 0 C1.58925224 -0.00374027 3.17850264 -0.0083433 4.76775014 -0.01373065 C9.11948553 -0.02592963 13.47114386 -0.02570384 17.82289243 -0.02334189 C22.52246353 -0.02293294 27.22201018 -0.03398046 31.92156982 -0.04345703 C41.12363436 -0.06005144 50.32567327 -0.06556589 59.52775152 -0.06668179 C67.01072655 -0.06763309 74.49369314 -0.07174748 81.9766655 -0.07808304 C103.20779746 -0.0956919 124.43890923 -0.10492257 145.67004855 -0.10342209 C147.38597477 -0.10330222 147.38597477 -0.10330222 149.13656616 -0.10317993 C150.85459586 -0.10305729 150.85459586 -0.10305729 152.60733323 -0.10293217 C171.16205994 -0.10211957 189.71670541 -0.12125155 208.27140863 -0.14945666 C227.33847101 -0.1782111 246.4054938 -0.19199552 265.47257841 -0.19026911 C276.17112177 -0.18960605 286.86958 -0.19506141 297.56810379 -0.21662521 C306.67851732 -0.23487078 315.78880711 -0.23915672 324.89923103 -0.22560637 C329.54314955 -0.2190935 334.18683092 -0.21889843 338.83072662 -0.23631287 C343.09097049 -0.25212357 347.35079075 -0.24909912 351.61102478 -0.2317446 C353.14331592 -0.22869218 354.67563931 -0.23223996 356.20789237 -0.24346475 C375.64402613 -0.37629407 393.66983665 5.07158741 408.36459351 18.26742554 C409.18572632 18.99574585 410.00685913 19.72406616 410.85287476 20.47445679 C423.41979273 32.27858763 432.57274895 49.15897956 433.55220103 66.61006474 C433.62831794 70.70888543 433.64118139 74.80351412 433.63201904 78.90283203 C433.63575932 80.49208427 433.64036234 82.08133467 433.64574969 83.67058218 C433.65794867 88.02231756 433.65772288 92.37397589 433.65536094 96.72572446 C433.65495198 101.42529556 433.66599951 106.12484221 433.67547607 110.82440186 C433.69207048 120.02646639 433.69758493 129.2285053 433.69870083 138.43058355 C433.69965213 145.91355858 433.70376652 153.39652517 433.71010208 160.87949753 C433.72771094 182.11062949 433.73694161 203.34174126 433.73544113 224.57288058 C433.73532126 226.2888068 433.73532126 226.2888068 433.73519897 228.03939819 C433.73511721 229.18475133 433.73503545 230.33010446 433.73495121 231.51016526 C433.73413862 250.06489197 433.75327059 268.61953744 433.78147571 287.17424066 C433.81023014 306.24130304 433.82401456 325.30832583 433.82228816 344.37541044 C433.8216251 355.0739538 433.82708045 365.77241203 433.84864426 376.47093582 C433.86688982 385.58134935 433.87117576 394.69163914 433.85762541 403.80206306 C433.85111254 408.44598158 433.85091747 413.08966295 433.86833191 417.73355865 C433.88414262 421.99380252 433.88111816 426.25362278 433.86376365 430.51385681 C433.86071122 432.04614795 433.864259 433.57847134 433.8754838 435.1107244 C434.00831311 454.54685816 428.56043163 472.57266868 415.36459351 487.26742554 C414.63627319 488.08855835 413.90795288 488.90969116 413.15756226 489.75570679 C401.35343141 502.32262476 384.47303948 511.47558098 367.0219543 512.45503306 C362.92313362 512.53114998 358.82850492 512.54401342 354.72918701 512.53485107 C353.13993477 512.53859135 351.55068437 512.54319437 349.96143687 512.54858172 C345.60970148 512.5607807 341.25804315 512.56055491 336.90629458 512.55819297 C332.20672348 512.55778401 327.50717683 512.56883154 322.80761719 512.57830811 C313.60555265 512.59490252 304.40351374 512.60041696 295.20143549 512.60153286 C287.71846047 512.60248416 280.23549387 512.60659855 272.75252151 512.61293411 C251.52138955 512.63054297 230.29027778 512.63977364 209.05913846 512.63827316 C207.34321224 512.6381533 207.34321224 512.6381533 205.59262085 512.63803101 C203.87459115 512.63790836 203.87459115 512.63790836 202.12185378 512.63778324 C183.56712707 512.63697065 165.0124816 512.65610263 146.45777838 512.68430774 C127.390716 512.71306217 108.32369321 512.7268466 89.25660861 512.72512019 C78.55806524 512.72445713 67.85960702 512.72991248 57.16108322 512.75147629 C48.05066969 512.76972185 38.9403799 512.77400779 29.82995598 512.76045744 C25.18603746 512.75394457 20.54235609 512.75374951 15.89846039 512.77116394 C11.63821652 512.78697465 7.37839626 512.78395019 3.11816223 512.76659568 C1.58587109 512.76354325 0.0535477 512.76709103 -1.47870536 512.77831583 C-20.91483912 512.91114514 -38.94064964 507.46326366 -53.63540649 494.26742554 C-54.45653931 493.53910522 -55.27767212 492.81078491 -56.12368774 492.06039429 C-68.69060572 480.25626344 -77.84356194 463.37587151 -78.82301402 445.92478633 C-78.89913093 441.82596565 -78.91199438 437.73133695 -78.90283203 433.63201904 C-78.90657231 432.04276681 -78.91117533 430.45351641 -78.91656268 428.8642689 C-78.92876166 424.51253351 -78.92853587 420.16087519 -78.92617393 415.80912662 C-78.92576497 411.10955552 -78.93681249 406.41000886 -78.94628906 401.71044922 C-78.96288347 392.50838468 -78.96839792 383.30634577 -78.96951382 374.10426752 C-78.97046512 366.6212925 -78.97457951 359.1383259 -78.98091507 351.65535355 C-78.99852393 330.42422158 -79.0077546 309.19310981 -79.00625412 287.96197049 C-79.00617421 286.81801968 -79.0060943 285.67406887 -79.00601196 284.49545288 C-79.0059302 283.35009975 -79.00584844 282.20474662 -79.0057642 281.02468581 C-79.0049516 262.4699591 -79.02408358 243.91531363 -79.0522887 225.36061041 C-79.08104313 206.29354803 -79.09482755 187.22652524 -79.09310114 168.15944064 C-79.09243808 157.46089727 -79.09789344 146.76243905 -79.11945724 136.06391525 C-79.13770281 126.95350173 -79.14198875 117.84321193 -79.1284384 108.73278801 C-79.12192553 104.08886949 -79.12173046 99.44518812 -79.1391449 94.80129242 C-79.15495561 90.54104856 -79.15193115 86.2812283 -79.13457663 82.02099426 C-79.13152421 80.48870312 -79.13507199 78.95637973 -79.14629678 77.42412667 C-79.2791261 57.98799291 -73.83124462 39.96218239 -60.63540649 25.26742554 C-59.90708618 24.44629272 -59.17876587 23.62515991 -58.42837524 22.77914429 C-42.51980493 5.8425565 -22.55010792 -0.05040155 0 0 Z " fill="#48B02C" transform="translate(78.63540649414063,-0.267425537109375)"/>
<path d="M0 0 C0.81339844 -0.01933594 1.62679688 -0.03867188 2.46484375 -0.05859375 C9.67908044 1.31916273 14.80754595 7.881373 19.75 12.875 C20.50861328 13.62587891 21.26722656 14.37675781 22.04882812 15.15039062 C22.77908203 15.88322266 23.50933594 16.61605469 24.26171875 17.37109375 C24.92002686 18.03165771 25.57833496 18.69222168 26.2565918 19.37280273 C29.87485945 23.50957298 31.38405406 26.61768945 31.55859375 32.1796875 C30.00165892 39.26557986 24.71332033 44.21586859 19.75039673 49.1305542 C19.01208945 49.87354145 18.27378217 50.61652871 17.51310194 51.38203073 C15.04527407 53.8617938 12.56689887 56.33073181 10.08837891 58.79980469 C8.31367275 60.57857714 6.53960518 62.35798691 4.76612854 64.13798523 C0.45242347 68.46394275 -3.86824843 72.78283114 -8.19243382 77.09830967 C-11.70705308 80.60614559 -15.21872327 84.11690866 -18.72832108 87.62976837 C-19.22838716 88.13029416 -19.72845325 88.63081994 -20.24367286 89.14651318 C-21.25961603 90.16341555 -22.27555248 91.18032465 -23.29148228 92.19724038 C-32.81045392 101.72475968 -42.33858698 111.24307417 -51.86989398 120.75825098 C-60.0447852 128.91973548 -68.21177321 137.08905925 -76.37304688 145.26416016 C-85.85642909 154.76360212 -95.34466564 164.2581518 -104.84068549 173.74496198 C-105.85347907 174.75679993 -106.86626639 175.76864414 -107.87904739 176.78049469 C-108.37732713 177.2783116 -108.87560686 177.77612851 -109.38898598 178.28903078 C-112.89706925 181.79452564 -116.40166316 185.30348574 -119.90491867 188.81380463 C-124.62066198 193.53884705 -129.34430215 198.25585319 -134.07336617 202.96756935 C-135.80857615 204.6988096 -137.54123275 206.4326134 -139.27115059 208.16914177 C-141.63104623 210.53722307 -143.99996892 212.89588877 -146.37142944 215.25238037 C-147.05548556 215.94312865 -147.73954167 216.63387693 -148.44432676 217.34555697 C-152.4861995 221.33802061 -156.35473626 224.93271198 -162.2109375 225.7578125 C-166.54414897 225.35509768 -169.31483633 224.49105637 -172.55215454 221.53152466 C-173.24615986 220.89730717 -173.94016518 220.26308968 -174.65520096 219.60965347 C-179.07436923 215.40375778 -183.41214448 211.11954266 -187.72802734 206.80810547 C-188.78980041 205.75097102 -189.85176677 204.69403069 -190.91390991 203.63726807 C-193.77548755 200.78870256 -196.63381973 197.93690328 -199.49113035 195.08405828 C-201.27840848 193.29972471 -203.06626626 191.51597391 -204.85429764 189.73239517 C-210.45541114 184.14491737 -216.05454813 178.55547026 -221.6507858 172.9631089 C-228.10009295 166.51829485 -234.55591194 160.080098 -241.01739919 153.6474961 C-246.02327004 148.66322669 -251.02337589 143.67322201 -256.01912653 138.67880929 C-258.99895974 135.70002376 -261.98117033 132.7237152 -264.96940231 129.75335312 C-267.77497189 126.96424096 -270.5733204 124.16808254 -273.36627579 121.36634064 C-274.39162779 120.34021744 -275.41954725 119.31665236 -276.45021057 118.29586411 C-277.85731283 116.90145705 -279.25428489 115.49684969 -280.65072632 114.09176636 C-281.43492132 113.30903791 -282.21911633 112.52630947 -283.02707481 111.71986198 C-286.00683107 108.15998304 -287.34762647 104.76284588 -287 100.125 C-286.1135946 94.30141183 -283.03052769 91.45378343 -278.9375 87.4375 C-277.72701181 86.18436962 -276.51889396 84.9289474 -275.3125 83.671875 C-273.98060667 82.30068313 -272.6472547 80.9309067 -271.3125 79.5625 C-270.34449463 78.56162354 -270.34449463 78.56162354 -269.35693359 77.54052734 C-265.4690282 73.62859163 -262.45002543 70.79695054 -256.76171875 70.21484375 C-252.25213771 70.55285622 -249.50668062 71.42025123 -246.14880371 74.50672913 C-245.4567131 75.14017305 -244.7646225 75.77361698 -244.05155945 76.42625618 C-240.60878977 79.70158945 -237.22061356 83.02366215 -233.86914062 86.39233398 C-232.72622055 87.53503502 -232.72622055 87.53503502 -231.56021118 88.70082092 C-229.06901004 91.1927128 -226.58135913 93.68811647 -224.09375 96.18359375 C-222.35662369 97.92258723 -220.61927491 99.6613585 -218.88171387 101.3999176 C-214.32903789 105.95639525 -209.77937419 110.515869 -205.23034668 115.07598877 C-200.57987108 119.7367292 -195.92653156 124.39460836 -191.2734375 129.05273438 C-182.15866475 138.17820296 -173.04704336 147.30681135 -163.9375 156.4375 C-159.26449253 152.53339069 -154.83673106 148.47698482 -150.54867554 144.1554718 C-149.9459402 143.55217975 -149.34320485 142.9488877 -148.7222048 142.32731408 C-146.72175487 140.32385047 -144.724908 138.31683512 -142.72802734 136.30981445 C-141.28708124 134.86530882 -139.84589759 133.42104012 -138.40449524 131.97698975 C-135.31041489 128.87637762 -132.2178493 125.77426536 -129.12641525 122.67101479 C-124.23814616 117.7644091 -119.34549361 112.86219036 -114.45205688 107.96073914 C-100.54110271 94.02566424 -86.63608471 80.08467476 -72.73535156 66.1394043 C-65.05094552 58.43045531 -57.36295407 50.72510008 -49.67109454 43.02358788 C-44.80533042 38.15128083 -39.94410825 33.27450077 -35.08619285 28.39436877 C-32.06597175 25.36242447 -29.04158435 22.3346633 -26.01599693 19.30807495 C-24.61339977 17.90332911 -23.21252627 16.49685983 -21.81350517 15.08855247 C-19.90437912 13.1672452 -17.98918424 11.25219707 -16.07235718 9.33857727 C-15.51972682 8.77943911 -14.96709647 8.22030096 -14.39771974 7.64421922 C-10.02249858 3.29744007 -6.34272175 0.0100201 0 0 Z " fill="#FDFEFD" transform="translate(383.9375,143.5625)"/>
</svg>`;
        html += '</span>';
        return html;
    }

    getProgressBarHtml() {
        if (!this.isProgressEnd())
            return `<div class="time-progress" style = "width: ${this.getProgressWidth()}">
                    <div class="time-progress-bar" 
                         style="width: ${this.getProgressBarWidth()}">
                    </div>
                    ${this.getGraduationsHtml()}
                </div>`;
        else
            return '';
    }

    getGraduationsHtml() {
        let max = this.max();
        let graduations = '';
        for (let i = 0; i < max - 1; i++) {
            const left = (i + 1) * 100 / max;
            graduations += `<div class="graduation-bar" style="left:${left}%"></div>`;
        }
        return graduations;
    }

    getProgressWidth() {
        let max = this.max();
        let maxFullWidth = this.maxFullWidth();
        return `${(max / maxFullWidth) * 100}%`;
    }

    getProgressBarWidth() {
        let current = this.current();
        let max = this.max();
        let width = current / max * 100;
        if (width < 1)
            width = 1;
        return `${width}%`;
    }

    caption() {
        return this.dataset['caption'] ?? '';
    }

    tooltip() {
        return this.dataset['tooltip'] ?? '';
    }

    max() {
        return parseInt(this.dataset['max'] ?? '0', 10);
    }

    maxFullWidth() {
        return parseInt(this.dataset['maxFullWidth'] ?? '0', 10);
    }

    current() {
        return parseFloat(this.dataset['current'] ?? '0');
    }

    numberMedication() {
        return parseInt(this.dataset['numberMedication'] ?? '0', 10);
    }
}

customElements.define('time-progress-bar', TimeProgressBar);