// utils/dateUtils.js
export const formatDateForAPI = (dateInput) => {
  if (!dateInput) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
  }

  if (typeof dateInput === 'string') {
    // Если это уже в формате YYYY-MM-DD
    if (/^\d{4}-\d{2}-\d{2}$/.test(dateInput)) {
      return dateInput;
    }

    // Пытаемся распарсить дату
    try {
      const date = new Date(dateInput);
      return date.toISOString().split('T')[0];
    } catch {
      return new Date().toISOString().split('T')[0];
    }
  }

  if (dateInput instanceof Date) {
    return dateInput.toISOString().split('T')[0];
  }

  return new Date().toISOString().split('T')[0];
};

export const formatDateForDisplay = (dateString) => {
  if (!dateString) return '';

  const date = new Date(dateString);
  return date.toLocaleDateString('ru-RU', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};

export const formatTime = (time) => {
  if (!time) return '';

  // Если время приходит в формате "HH:mm:ss"
  if (typeof time === 'string') {
    return time.substring(0, 5);
  }

  // Если это объект TimeOnly из C# (может приходить в разных форматах)
  return String(time);
};

export const getDatePresets = () => {
  const today = new Date();
  return [
    { label: 'Сегодня', date: new Date(today) },
    { label: 'Завтра', date: new Date(today.getTime() + 86400000) },
    { label: 'Послезавтра', date: new Date(today.getTime() + 172800000) },
    { label: 'Через неделю', date: new Date(today.getTime() + 604800000) }
  ];
};
