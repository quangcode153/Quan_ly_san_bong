using BTLtruc_quan.Shared.DTOs;

namespace BTLtruc_quan.WinForms.Controls.Grids;

/// <summary>
/// Dữ liệu sự kiện khi người dùng click vào một ô ca sân trên ma trận lưới
/// </summary>
public class CourtCellClickedEventArgs : EventArgs
{
    public CourtTimeSlotCellDto Cell { get; }
    public CourtGridRowDto Row { get; }
    public TimeSlotDto TimeSlot { get; }
    public MouseButtons Button { get; }

    public CourtCellClickedEventArgs(CourtTimeSlotCellDto cell, CourtGridRowDto row, TimeSlotDto timeSlot, MouseButtons button)
    {
        Cell = cell;
        Row = row;
        TimeSlot = timeSlot;
        Button = button;
    }
}

/// <summary>
/// Dữ liệu sự kiện khi danh sách các ô ca sân được chọn thay đổi
/// </summary>
public class CourtSelectionChangedEventArgs : EventArgs
{
    public IReadOnlyList<CourtTimeSlotCellDto> SelectedCells { get; }

    public CourtSelectionChangedEventArgs(IReadOnlyList<CourtTimeSlotCellDto> selectedCells)
    {
        SelectedCells = selectedCells;
    }
}
