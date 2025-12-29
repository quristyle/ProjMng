using System;
using System.Collections.Generic;

namespace skgRestApi.Models;
    /// <summary>생일자 관리 항목 (Present Day)</summary>
    public class BirthdayEntry : BaseEntity
    {
        /// <summary>사원/회원 식별자</summary>
        public string SubjectId { get; set; } = string.Empty;

        /// <summary>이름</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>생년월일 (양력)</summary>
        public DateOnly BirthDate { get; set; }

        /// <summary>음력 여부</summary>
        public bool IsLunar { get; set; }

        /// <summary>사내 발표/기념 대상 여부</summary>
        public bool IsCelebrated { get; set; }
    }

    /// <summary>식단 안내 - 식단 항목</summary>
    public class MealMenuItem : BaseEntity
    {
        /// <summary>메뉴 이름</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>설명/재료 정보</summary>
        public string? Description { get; set; }

        /// <summary>칼로리 (선택)</summary>
        public int? Calories { get; set; }

        /// <summary>알레르기 정보 (콤마 구분)</summary>
        public string? AllergyInfo { get; set; }
    }

    /// <summary>일자별 식단 (식단 안내)</summary>
    public class MealMenu : BaseEntity
    {
        /// <summary>식단 적용 일자</summary>
        public DateOnly Date { get; set; }

        /// <summary>식사 유형 (아침/점심/저녁/간식)</summary>
        public MealType Type { get; set; }

        /// <summary>메뉴 항목 목록</summary>
        public List<MealMenuItem> Items { get; set; } = new();
    }

  /// <summary>식사 유형 (아침/점심/저녁/간식)</summary>
  public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }

    /// <summary>식단 의견 커뮤니티</summary>
    public class MealFeedback : BaseEntity
    {
        /// <summary>연관 식단 ID (MealMenu.Id)</summary>
        public int MealMenuId { get; set; }

        /// <summary>작성자 식별자</summary>
        public string AuthorId { get; set; } = string.Empty;

        /// <summary>의견 내용</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>평점 (1-5, 선택)</summary>
        public int? Rating { get; set; }

        /// <summary>좋아요 수</summary>
        public int Likes { get; set; }
    }

    /// <summary>날씨 정보</summary>
    public class WeatherInfo : BaseEntity
    {
        /// <summary>관측 지역 (예: 본사, 지사 코드 등)</summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>관측 시각(UTC 권장)</summary>
        public DateTime ObservationTime { get; set; } = DateTime.UtcNow;

        /// <summary>섭씨 온도</summary>
        public double TemperatureC { get; set; }

        /// <summary>기상 상태 설명 (맑음/흐림/비 등)</summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>습도(%)</summary>
        public int? Humidity { get; set; }

        /// <summary>풍속 (m/s)</summary>
        public double? WindSpeed { get; set; }

        /// <summary>외부 소스 아이콘/이미지 URL</summary>
        public string? IconUrl { get; set; }
    }

    /// <summary>사내 시스템 허브(Connect Hub) 항목</summary>
    public class ConnectHubItem : BaseEntity
    {
        /// <summary>표시 제목</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>연결 URL 또는 라우트</summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>카테고리(예: 포털, 문서, 툴)</summary>
        public string? Category { get; set; }

        /// <summary>외부 링크 여부</summary>
        public bool IsExternal { get; set; }

        /// <summary>정렬 우선순위</summary>
        public int Order { get; set; }
    }

    /// <summary>My SFM (Safety Feedback Measurement) 보고서</summary>
    public class SfmReport : BaseEntity
    {
        /// <summary>보고자 ID</summary>
        public string ReporterId { get; set; } = string.Empty;

        /// <summary>보고일시</summary>
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        /// <summary>측정 점수(예: 0.0 ~ 100.0)</summary>
        public decimal MeasurementScore { get; set; }

        /// <summary>피드백 내용</summary>
        public string? Feedback { get; set; }

        /// <summary>카테고리(안전/위험요소 등)</summary>
        public string? Category { get; set; }

        /// <summary>처리 여부</summary>
        public bool IsAcknowledged { get; set; }

        /// <summary>처리 일시</summary>
        public DateTime? AcknowledgedAt { get; set; }
    }

    /// <summary>안전기준정보</summary>
    public class SafetyStandard : BaseEntity
    {
        /// <summary>기준 코드(유니크)</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>제목</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>요약</summary>
        public string? Summary { get; set; }

        /// <summary>상세 내용 (마크다운 또는 HTML 가능)</summary>
        public string? Details { get; set; }

        /// <summary>버전</summary>
        public string? Version { get; set; }

        /// <summary>시행 일자</summary>
        public DateOnly? EffectiveDate { get; set; }
    }

    /// <summary>안전스티커 발부 기록</summary>
    public class SafetySticker : BaseEntity
    {
        /// <summary>스티커 코드 (예: STK-0001)</summary>
        public string StickerCode { get; set; } = string.Empty;

        /// <summary>수령자 식별자</summary>
        public string RecipientId { get; set; } = string.Empty;

        /// <summary>발급 일시</summary>
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        /// <summary>유효 일시 (없을 수 있음)</summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>발급 사유</summary>
        public string? Reason { get; set; }

        /// <summary>발급자 ID</summary>
        public string? IssuedBy { get; set; }
    }

    /// <summary>안전위반 모니터링(위반 기록)</summary>
    public class SafetyViolation : BaseEntity
    {
        /// <summary>위반자/대상 ID</summary>
        public string ViolatorId { get; set; } = string.Empty;

        /// <summary>관찰/발생 시각</summary>
        public DateTime ObservedAt { get; set; } = DateTime.UtcNow;

        /// <summary>위치(구역)</summary>
        public string? Location { get; set; }

        /// <summary>위반 유형</summary>
        public string ViolationType { get; set; } = string.Empty;

        /// <summary>심각도(1-5)</summary>
        public int Severity { get; set; }

        /// <summary>상세 설명</summary>
        public string? Description { get; set; }

        /// <summary>해결 여부</summary>
        public bool IsResolved { get; set; }

        /// <summary>해결 일시</summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>조치 내용</summary>
        public string? ActionTaken { get; set; }
    }

    /// <summary>환경설정 (애플리케이션 설정)</summary>
    public class AppSetting : BaseEntity
    {
        /// <summary>설정 키 (유니크)</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>설정 값 (문자열로 저장)</summary>
        public string? Value { get; set; }

        /// <summary>섹션/그룹</summary>
        public string? Section { get; set; }

        /// <summary>값 타입(예: String, Int, Bool, Json)</summary>
        public string? DataType { get; set; }

        /// <summary>민감 정보 여부 (암호화/마스킹 필요)</summary>
        public bool IsSensitive { get; set; }
    }

    /// <summary>프로필 관리</summary>
    public class UserProfile : BaseEntity
    {
        /// <summary>사용자/사원 ID</summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>성명</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>이메일</summary>
        public string? Email { get; set; }

        /// <summary>전화번호</summary>
        public string? Phone { get; set; }

        /// <summary>부서</summary>
        public string? Department { get; set; }

        /// <summary>직책</summary>
        public string? Position { get; set; }

        /// <summary>사진 URL</summary>
        public string? PhotoUrl { get; set; }

        /// <summary>사용자 선호 설정 (JSON)</summary>
        public string? PreferencesJson { get; set; }
    }

    /// <summary>기준관리 (참조 값)</summary>
    public class ReferenceStandard : BaseEntity
    {
        /// <summary>키/코드</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>기준명</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>숫자 값(있을 경우)</summary>
        public decimal? NumericValue { get; set; }

        /// <summary>단위</summary>
        public string? Unit { get; set; }

        /// <summary>설명</summary>
        public string? Description { get; set; }
    }
