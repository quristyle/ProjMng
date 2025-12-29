향후 진행은 다음과 같이 하려고 합니다.

- 단가계약 체결 (IT유지보수 계약과 동시 진행 중)

- 세부 구현 내용 관련 관련자 회의 -> 세부내용 확정

- 세부내용에 대한 견적 금액 산정

- 지허브 내부 품의완료 후 작업 요청

세부 내역 관련해서는 회의때 이야기되겠지만,,

- 아래와 같은 컨셉이라 SHE현장관리 부분을 앞으로,

- 생일자는 다른 시스템과 연동할 필요는 없어 보임 등등..

- 아래 내용을 바탕으로 AI 로 임시로 화면구성해 본 내용입니다.

  https://gemini.google.com/share/1f348d3af781

그리고, 위반자에게는 아래 뿌리오라는 사이트를 통해 문자메시지나 카카오톡으로 보낼 수 있을 것 같습니다.

https://www.bizppurio.com/

관리자 ID: ghub3650 ,PW: ghub1004\*\*

---

Concept : SHE 현장관리 어플

화면 구성

[SHE 현장관리]

1. 날씨 정보 : 풍속, 온도 등 -> SHE관련 기준 초과시 강조

2. 골든룰 위반 통지서 발행 -> 첨부 참조

   - 골든룰 위반 조치기준

   - 골든룰 위반 통지서 -> 작성 후 카톡에 공유할 수 있는 버튼 필요

3. 생각중....

   - chatGPTs 링크

   - SHE관련법률 사이트 링크

   - SHE규정/절차서 사이트 링크

   - mySFM 점수 확인 -> 매달 엑셀자료 업로드

[Connect Hub]

1. 오늘 생일자 -> 미리 업로드, 삭제/추가 필요

2. 이번 주 식당 메뉴 -> 그림파일로 매주 업로드

3. 업무 사이트 링크

   1. 그룹웨어 앱

   2. 구매시스템 사이트

   3. Ocean-H 사이트

   4. mySUNI 앱

   5. Library 지관 사이트

   6. ......

이준호 드림

날씨 api

기상청 api (구 동네예보 조회 서비스)
https://www.data.go.kr/index.do
개발기 기준 일일 트래픽 10,000

https://www.data.go.kr/data/15084084/openapi.do

과학기술 기상청

활용신청
[승인] 기상청*단기예보 ((구)*동네예보) 조회서비스

계정 개발 신청일 2025-11-25 만료예정일 2027-11-25

https://www.data.go.kr/data/15084084/openapi.do#tab_layer_detail_function

참고문서 기상청41*단기예보 조회서비스*오픈API활용가이드\_2510.zip
데이터포맷 JSON+XML
End Point https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0
API 환경 또는 API 호출 조건에 따라 인증키가 적용되는 방식이 다를 수 있습니다.
포털에서 제공되는 Encoding/Decoding 된 인증키를 적용하면서 구동되는 키를 사용하시기 바랍니다.

- 향후 포털에서 더 명확한 정보를 제공하기 위해 노력하겠습니다.
  일반 인증키
  19f81d91ba4065e5f6e8cd744be132d3f48491828393b94d022a10603675637f

https://www.data.go.kr/data/15084084/openapi.do

활용승인 절차 개발단계 : 자동승인 / 운영단계 : 자동승인
신청가능 트래픽 개발계정 : 10,000 / 운영계정 : 활용사례 등록시 신청하면 트래픽 증가 가능
요청주소 http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst
서비스URL http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0

요청변수(Request Parameter)

항목명(국문) 항목명(영문) 항목크기 항목구분 샘플데이터 항목설명
서비스키 ServiceKey 4 필수 - 공공데이터포털에서 받은 인증키
페이지 번호 pageNo 4 필수 1 페이지번호
한 페이지 결과 수 numOfRows 4 필수 1000 한 페이지 결과 수
응답자료형식 dataType 4 옵션 XML 요청자료형식(XML/JSON) Default: XML
발표일자 base_date 8 필수 20210628 ‘21년 6월 28일 발표
발표시각 base_time 4 필수 0600 06시 발표(정시단위)
예보지점 X 좌표 nx 2 필수 55 예보지점의 X 좌표값
예보지점 Y 좌표 ny 2 필수 127 예보지점의 Y 좌표값
출력결과(Response Element)

항목명(국문) 항목명(영문) 항목크기 항목구분 샘플데이터 항목설명
결과코드 resultCode 2 필수 00 결과코드
결과메시지 resultMsg 50 필수 OK 결과메시지
한 페이지 결과 수 numOfRows 4 필수 10 한 페이지 결과 수
페이지 번호 pageNo 4 필수 1 페이지번호
전체 결과 수 totalCount 4 필수 3 전체 결과 수
데이터 타입 dataType 4 필수 XML 응답자료형식 (XML/JSON)
발표일자 baseDate 8 필수 20210628 ‘21년 6월 28일 발표
발표시각 baseTime 6 필수 0600 06시 발표(매 정시)
예보지점 X 좌표 nx 2 필수 55 입력한 예보지점 X 좌표
예보지점 Y 좌표 ny 2 필수 127 입력한 예보지점 Y 좌표
자료구분코드 category 3 필수 RN1 자료구분코드
실황 값 obsrValue 2 필수 0 RN1, T1H, UUU, VVV, WSD 실수로 제공

아큐웨더
https://developer.accuweather.com/
50 calls/day
OpenWeather
https://openweathermap.org/
60 calls/minute, 1,000,000 calls/month
